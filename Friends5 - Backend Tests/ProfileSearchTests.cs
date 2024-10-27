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

            var profileInfo = new ReceivedProfileData { DateOfBirth = new DateOnly(2002, 1, 15), Gender = Gender.Male };
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
            Assert.Equal(2002, profileData!.DateOfBirth!.Value.Year);
            Assert.Equal(1, profileData.DateOfBirth!.Value.Month);
            Assert.Equal(15, profileData.DateOfBirth!.Value.Day);
            Assert.Equal(Gender.Male, profileData.Gender);
        }
    }
}
