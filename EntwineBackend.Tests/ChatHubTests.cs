using EntwineBackend;
using EntwineBackend.DbItems;
using EntwineBackend.Data;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Text;
using System.Text.Json;

namespace EntwineBackend_Tests
{
    [Collection("TestProgram collection")]
    public class ChatHubTests
    {
        private readonly HttpClient _client;
        private readonly TestServer _server;

        public ChatHubTests(TestProgram factory)
        {
            _client = factory.Client;
            _server = factory.Server;
        }

        [Fact]
        public async Task ReceiveMessageFromHub()
        {
            var message = "";

            // This was set in LoginAsUser in setup. Remove "Bearer " from the header to get the token
            var token = _client.DefaultRequestHeaders.Authorization!.ToString()["Bearer ".Length..];

            var connection = new HubConnectionBuilder()
                .WithUrl($"{_client.BaseAddress}chatHub?chatId=1", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult<string?>(token);
                    options.HttpMessageHandlerFactory = _ => _server.CreateHandler();
                })
                .Build();
            await connection.StartAsync();
            Assert.Equal(HubConnectionState.Connected, connection.State);

            var tcs = new TaskCompletionSource<bool>();
            connection.On<Message>("ReceiveMessage", messageReceived =>
            {
                message = messageReceived.Content;
                tcs.SetResult(true);
            });

            var requestUrl = "/Chat/1/Messages";
            var messageData = new MessageToSend { Content = "Hello" };
            var json = JsonSerializer.Serialize(messageData, DefaultJsonOptions.Instance);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(requestUrl, httpContent);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            await tcs.Task;

            Assert.Equal("Hello", message);
        }

        [Fact]
        public async Task CannotConnectToChatUserIsNotPartOf()
        {
            try
            {
                await TestUtils.LoginAsUser(_client, "SomeUsername3");

                var token = _client.DefaultRequestHeaders.Authorization!.ToString()["Bearer ".Length..];

                var connection = new HubConnectionBuilder()
                    .WithUrl($"{_client.BaseAddress}chatHub?chatId=1", options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult<string?>(token);
                        options.HttpMessageHandlerFactory = _ => _server.CreateHandler();
                    })
                    .Build();
                await Assert.ThrowsAsync<HttpRequestException>(async () => await connection.StartAsync());
            }
            finally
            {
                await TestUtils.LoginAsUser(_client, "SomeUsername1");
            }
        }
    }
}
