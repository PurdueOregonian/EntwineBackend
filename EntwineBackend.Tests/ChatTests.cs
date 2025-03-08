using EntwineBackend;
using EntwineBackend.DbItems;
using EntwineBackend.Data;
using System.Net;
using System.Text;
using System.Text.Json;

namespace EntwineBackend_Tests
{
    [Collection("TestProgram collection")]
    public class ChatTests
    {
        private readonly HttpClient _client;

        public ChatTests(TestProgram factory)
        {
            _client = factory.Client;
        }

        [Fact]
        public async Task AddChatAndSendMessage()
        {
            try
            {
                var requestUrl = "/Chat";

                var response = await _client.GetAsync(requestUrl);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var responseContent = await response.Content.ReadAsStringAsync();
                var chats = JsonSerializer.Deserialize<List<ChatData>>(responseContent, DefaultJsonOptions.Instance);
                Assert.Single(chats!);
                Assert.Equal("SomeUsername2", chats![0].Usernames!.Single());

                requestUrl = "/Chat/1/Messages";
                var messageData = new MessageToSend { Content = "Hello" };
                var json = JsonSerializer.Serialize(messageData, DefaultJsonOptions.Instance);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                response = await _client.PostAsync(requestUrl, httpContent);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                responseContent = await response.Content.ReadAsStringAsync();
                var newMessage = JsonSerializer.Deserialize<MessageReturnData>(responseContent, DefaultJsonOptions.Instance);
                Assert.Equal("SomeUsername1", newMessage!.Username);
                Assert.Contains("Hello", newMessage.Content);

                await TestUtils.LoginAsUser(_client, "SomeUsername2");
                response = await _client.GetAsync(requestUrl);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                responseContent = await response.Content.ReadAsStringAsync();
                var messages = JsonSerializer.Deserialize<List<MessageReturnData>>(responseContent, DefaultJsonOptions.Instance);
                // ChatHubTests could have sent a message also. Just check the last message
                Assert.Equal("SomeUsername1", messages![^1].Username);
                Assert.Contains("Hello", messages[^1].Content);
            }
            finally
            {
                await TestUtils.LoginAsUser(_client, "SomeUsername1");
            }
        }
    }
}
