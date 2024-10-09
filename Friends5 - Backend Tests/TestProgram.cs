using Microsoft.AspNetCore.Mvc.Testing;

namespace Friends5___Backend_Tests
{
    public class TestProgram
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        public HttpClient Client => _client;

        public TestProgram()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseSetting("ConnectionStrings:DefaultConnection", "Host=localhost;Port=5433;Username=postgres;Password=eleanordonahue;Database=postgres");
                });
            _client = _factory.CreateClient();
        }
    }
}
