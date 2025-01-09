using EntwineBackend;
using EntwineBackend.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

namespace EntwineBackend_Tests
{
    public class TestProgram : IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly DockerManager _dockerManager;
        public HttpClient Client => _client;

        public TestProgram()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseSetting("ConnectionStrings:DefaultConnection", "Host=localhost;Port=5433;Username=postgres;Password=eleanordonahue;Database=postgres");
                });
            _client = _factory.CreateClient();
            _dockerManager = new DockerManager();
            _dockerManager.StartPostgresContainerAsync("eleanordonahue").Wait();
        }

        public async Task InitializeAsync()
        {
            var requestUrl = "/Auth/Register";
            string json;
            StringContent httpContent;
            HttpResponseMessage response;

            var profileInfos = new List<ReceivedProfileData>
            {
                new() { DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-24)), Gender = Gender.Female, Interests = [1] },
                new() { DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-31)), Gender = Gender.Male, Interests = [1] },
                new() { DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-44)), Gender = Gender.Other, Interests = [1] },
                new() { DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-52)), Gender = Gender.Male, Interests = [1] },
                new() { DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-63)), Gender = Gender.Female, Interests = [1] }
            };

            for (var i = 1; i <= 5; i++)
            {
                requestUrl = "/Auth/Register";
                var loginInfo = new LoginInfo { Username = $"SomeUsername{i}", Password = "SomePassword5@" };
                json = JsonSerializer.Serialize(loginInfo, DefaultJsonOptions.Instance);
                httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                response = await _client.PostAsync(requestUrl, httpContent);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                await TestUtils.LoginAsUser(_client, $"SomeUsername{i}");

                var profileInfo = profileInfos[i - 1];
                json = JsonSerializer.Serialize(profileInfo, DefaultJsonOptions.Instance);
                httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                requestUrl = "/Profile/Save";
                response = await _client.PostAsync(requestUrl, httpContent);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            await TestUtils.LoginAsUser(_client, "SomeUsername1");

            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            _client.Dispose();
            _factory.Dispose();
            _dockerManager.Dispose();
            await Task.CompletedTask;
        }
    }
}
