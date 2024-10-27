using Friends5___Backend;
using Friends5___Backend.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

namespace Friends5___Backend_Tests
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

            for(var i = 1; i <= 5; i++)
            {
                var loginInfo = new LoginInfo { Username = $"SomeUsername{i}", Password = "SomePassword5@" };
                var json = JsonSerializer.Serialize(loginInfo, DefaultJsonOptions.Instance);
                var httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(requestUrl, httpContent);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

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
