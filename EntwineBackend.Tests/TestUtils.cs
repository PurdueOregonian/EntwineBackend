using EntwineBackend;
using EntwineBackend.Authentication;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EntwineBackend_Tests
{
    internal class TestUtils
    {
        public static async Task LoginAsUser(HttpClient client, string username)
        {
            var loginInfo = new LoginInfo { Username = username, Password = "SomePassword5@" };
            var json = JsonSerializer.Serialize(loginInfo, DefaultJsonOptions.Instance);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var requestUrl = "/Auth/Login";

            var response = await client.PostAsync(requestUrl, httpContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<LoginResult>(responseContent, DefaultJsonOptions.Instance);

            var token = loginResult?.AccessToken ?? throw new Exception();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
