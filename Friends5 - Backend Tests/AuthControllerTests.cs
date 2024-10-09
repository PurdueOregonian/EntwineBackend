using Friends5___Backend.Authentication;
using System.Net;
using System.Text.Json;

namespace Friends5___Backend_Tests
{
    public class AuthControllerTests : IClassFixture<TestProgram>, IAsyncLifetime
    {
        private readonly HttpClient _client;

        public AuthControllerTests(TestProgram factory)
        {
            _client = factory.Client;
        }

        public async Task InitializeAsync()
        {
            var loginInfo = new LoginInfo { Username = "SomeUsername", Password = "SomePassword5@" };
            var json = JsonSerializer.Serialize(loginInfo);
            var httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var requestUrl = "/Auth/Register";

            var response = await _client.PostAsync(requestUrl, httpContent);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Login()
        {
            var loginInfo = new LoginInfo { Username = "SomeUsername", Password = "SomePassword5@" };
            var json = JsonSerializer.Serialize(loginInfo);
            var httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var requestUrl = "/Auth/Login";

            var response = await _client.PostAsync(requestUrl, httpContent);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData(null, "SomePassword5@")]
        [InlineData("SomeUsername", null)]
        [InlineData("SomeWrongUsername", "SomePassword5@")]
        [InlineData("SomeUsername", "SomeWrongPassword5@")]
        public async Task Login_UsernameOrPasswordNullOrIncorrect(string? username, string? password)
        {
            var loginInfo = new LoginInfo { Username = username, Password = password };
            var json = JsonSerializer.Serialize(loginInfo);
            var httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var requestUrl = "/Auth/Login";

            var response = await _client.PostAsync(requestUrl, httpContent);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
