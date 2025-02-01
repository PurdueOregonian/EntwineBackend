using EntwineBackend;
using EntwineBackend.DbItems;
using EntwineBackend.Data;
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
                var community = JsonSerializer.Deserialize<CommunityData>(responseContent, DefaultJsonOptions.Instance);
                Assert.Equal(TestConstants.TestLocation.Country, community!.Country);
                Assert.Equal(TestConstants.TestLocation.State, community.State);
                Assert.Equal(TestConstants.TestLocation.City, community.City);
                Assert.Equal([1, 2, 3], community.UserIds);
                Assert.Equal(Constants.InterestCategories.Length, community.Chats!.Count);
                Assert.True(community.Chats.Count(c => c.Name == Constants.InterestCategories[0].Name) == 1);

                await TestUtils.LoginAsUser(_client, "SomeUsername4");
                response = await _client.GetAsync(requestUrl);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                responseContent = await response.Content.ReadAsStringAsync();
                community = JsonSerializer.Deserialize<CommunityData>(responseContent, DefaultJsonOptions.Instance);
                Assert.Equal(TestConstants.TestLocation2.Country, community!.Country);
                Assert.Equal(TestConstants.TestLocation2.State, community.State);
                Assert.Equal(TestConstants.TestLocation2.City, community.City);
                Assert.Equal([4, 5], community.UserIds);
                Assert.Equal(Constants.InterestCategories.Length, community.Chats!.Count);
                Assert.True(community.Chats.Count(c => c.Name == Constants.InterestCategories[0].Name) == 1);
            }
            finally
            {
                await TestUtils.LoginAsUser(_client, "SomeUsername1");
            }
        }
    }
}
