using EntwineBackend;
using EntwineBackend.Authentication;
using System.Net;
using System.Text.Json;

namespace EntwineBackend_Tests
{
    [Collection("TestProgram collection")]
    public class AuthControllerTests
    {
        private readonly HttpClient _client;

        public AuthControllerTests(TestProgram factory)
        {
            _client = factory.Client;
        }

        [Fact]
        public async Task Login()
        {
            var loginInfo = new LoginInfo { Username = "SomeUsername1", Password = "SomePassword5@" };
            var json = JsonSerializer.Serialize(loginInfo, DefaultJsonOptions.Instance);
            var httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var requestUrl = "/Auth/Login";

            var response = await _client.PostAsync(requestUrl, httpContent);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData(null, "SomePassword5@")]
        [InlineData("SomeUsername1", null)]
        [InlineData("SomeWrongUsername", "SomePassword5@")]
        [InlineData("SomeUsername1", "SomeWrongPassword5@")]
        public async Task Login_UsernameOrPasswordNullOrIncorrect(string? username, string? password)
        {
            var loginInfo = new LoginInfo { Username = username, Password = password };
            var json = JsonSerializer.Serialize(loginInfo, DefaultJsonOptions.Instance);
            var httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var requestUrl = "/Auth/Login";

            var response = await _client.PostAsync(requestUrl, httpContent);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
