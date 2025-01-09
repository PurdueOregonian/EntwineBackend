using EntwineBackend;
using EntwineBackend.DbItems;
using System.Net;
using System.Text;
using System.Text.Json;

namespace EntwineBackend_Tests
{
    [Collection("TestProgram collection")]
    public class ProfileSearchTests
    {
        private readonly HttpClient _client;

        public ProfileSearchTests(TestProgram factory)
        {
            _client = factory.Client;
        }

        [Fact]
        public async Task Get()
        {
            try
            {
                var requestUrl = "/Profile";
                var response = await _client.GetAsync(requestUrl);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var responseContent = await response.Content.ReadAsStringAsync();
                var profileData = JsonSerializer.Deserialize<ProfileData>(responseContent, DefaultJsonOptions.Instance);
                Assert.Equal(DateTime.Now.Year - 24, profileData!.DateOfBirth!.Value.Year);
                Assert.Equal(DateTime.Now.Month, profileData.DateOfBirth!.Value.Month);
                Assert.Equal(DateTime.Now.Day, profileData.DateOfBirth!.Value.Day);
                Assert.Equal(Gender.Female, profileData.Gender);

                await TestUtils.LoginAsUser(_client, "SomeUsername2");

                requestUrl = "/Profile/SomeUsername1";
                response = await _client.GetAsync(requestUrl);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                responseContent = await response.Content.ReadAsStringAsync();
                var otherProfileData = JsonSerializer.Deserialize<ProfileSearchReturnData>(responseContent, DefaultJsonOptions.Instance);
                Assert.Equal(24, otherProfileData!.Age);
                Assert.Equal(Gender.Female, otherProfileData.Gender);
            }
            finally
            {
                await TestUtils.LoginAsUser(_client, "SomeUsername1");
            }
        }

        [Fact]
        public async Task Search()
        {
            try
            {
                await TestUtils.LoginAsUser(_client, "SomeUsername2");

                var requestUrl = "/Search";
                var content = new StringContent(JsonSerializer.Serialize(new
                {
                    MinAge = 18,
                    MaxAge = 40,
                    Gender = new List<Gender> { Gender.Female }
                }), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(requestUrl, content);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var responseContent = await response.Content.ReadAsStringAsync();
                var searchResults = JsonSerializer.Deserialize<List<ProfileSearchReturnData>>(responseContent, DefaultJsonOptions.Instance);
                Assert.NotNull(searchResults);
                Assert.NotEmpty(searchResults);

                content = new StringContent(JsonSerializer.Serialize(new
                {
                    MinAge = 30,
                    MaxAge = 40,
                    Gender = new List<Gender> { Gender.Female }
                }), Encoding.UTF8, "application/json");
                response = await _client.PostAsync(requestUrl, content);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                responseContent = await response.Content.ReadAsStringAsync();
                searchResults = JsonSerializer.Deserialize<List<ProfileSearchReturnData>>(responseContent, DefaultJsonOptions.Instance);
                Assert.NotNull(searchResults);
                Assert.Empty(searchResults);
            }
            finally
            {
                await TestUtils.LoginAsUser(_client, "SomeUsername1");
            }
        }
    }
}
