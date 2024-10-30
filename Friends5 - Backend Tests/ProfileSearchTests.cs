using Friends5___Backend;
using System.Net;
using System.Text.Json;

namespace Friends5___Backend_Tests
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
        public async Task SaveAndGet()
        {
            await TestUtils.LoginAsUser(_client, "SomeUsername1");

            var profileInfo = new ReceivedProfileData { DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-24)), Gender = Gender.Female };
            var json = JsonSerializer.Serialize(profileInfo, DefaultJsonOptions.Instance);
            var httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var requestUrl = "/Profile/Save";
            var response = await _client.PostAsync(requestUrl, httpContent);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            requestUrl = "/Profile";
            response = await _client.GetAsync(requestUrl);
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
            var otherProfileData = JsonSerializer.Deserialize<OtherProfileData>(responseContent, DefaultJsonOptions.Instance);
            Assert.Equal(24, otherProfileData!.Age);
            Assert.Equal(Gender.Female, otherProfileData.Gender);
        }
    }
}
