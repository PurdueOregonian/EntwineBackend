using EntwineBackend;
using EntwineBackend.DbItems;
using Friends5___Backend.Data;
using System.Net;
using System.Text;
using System.Text.Json;

namespace EntwineBackend_Tests
{
    [Collection("TestProgram collection")]
    public class CommunityTests
    {
        private readonly HttpClient _client;

        public CommunityTests(TestProgram factory)
        {
            _client = factory.Client;
        }

        [Fact]
        public async Task GetCommunity()
        {
            try
            {
                var requestUrl = "/Community";

                var response = await _client.GetAsync(requestUrl);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var responseContent = await response.Content.ReadAsStringAsync();
                var community = JsonSerializer.Deserialize<Community>(responseContent, DefaultJsonOptions.Instance);
                Assert.Equal(1, community!.Id);
                Assert.Equal(1, community.Location);
                Assert.Equal([1, 2, 3], community.UserIds);

                await TestUtils.LoginAsUser(_client, "SomeUsername4");
                response = await _client.GetAsync(requestUrl);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                responseContent = await response.Content.ReadAsStringAsync();
                community = JsonSerializer.Deserialize<Community>(responseContent, DefaultJsonOptions.Instance);
                Assert.Equal(2, community!.Id);
                Assert.Equal(2, community.Location);
                Assert.Equal([4, 5], community.UserIds);
            }
            finally
            {
                await TestUtils.LoginAsUser(_client, "SomeUsername1");
            }
        }
    }
}
