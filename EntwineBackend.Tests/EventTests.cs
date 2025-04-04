﻿using EntwineBackend;
using EntwineBackend.DbItems;
using EntwineBackend.Data;
using System.Net;
using System.Text;
using System.Text.Json;

namespace EntwineBackend_Tests
{
    [Collection("TestProgram collection")]
    public class EventTests
    {
        private readonly HttpClient _client;

        public EventTests(TestProgram factory)
        {
            _client = factory.Client;
        }

        [Fact]
        public async Task CreateAndGetEvent()
        {
            try
            {
                var createEventData = new CreateEventData {
                    Start = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    End = new DateTime(2030, 1, 1, 4, 0, 0, DateTimeKind.Utc),
                    Title = "Some Event",
                    MaxParticipants = 10
                };
                var json = JsonSerializer.Serialize(createEventData, DefaultJsonOptions.Instance);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var requestUrl = "/Events";

                var response = await _client.PostAsync(requestUrl, httpContent);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var responseContent = await response.Content.ReadAsStringAsync();
                var newEvent = JsonSerializer.Deserialize<Event>(responseContent, DefaultJsonOptions.Instance);
                Assert.Equal("Some Event", newEvent!.Title);

                requestUrl = "/Events";
                response = await _client.GetAsync(requestUrl);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                responseContent = await response.Content.ReadAsStringAsync();
                var events = JsonSerializer.Deserialize<List<Event>>(responseContent, DefaultJsonOptions.Instance);
                Assert.Single(events!);
                Assert.Equal(1, events![0].Community);
                Assert.Equal("Some Event", events![0].Title);
                Assert.Equal(2030, events[0].Start.Year);
                Assert.Equal(0, events[0].Start.Hour);
                Assert.Equal(2030, events[0].End.Year);
                Assert.Equal(4, events[0].End.Hour);
            }
            finally
            {
                await TestUtils.LoginAsUser(_client, "SomeUsername1");
            }
        }
    }
}
