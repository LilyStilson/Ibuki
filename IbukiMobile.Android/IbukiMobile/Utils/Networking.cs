using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IbukiMobile.Utils {
    public static class Networking {
        public static async Task<string> GetHTML(string URL) {
            HttpClient Client = new HttpClient();
            Uri URI = new Uri(URL);

            HttpResponseMessage Response = await Client.GetAsync(URI);

            return await Response.Content.ReadAsStringAsync();
        }

        public static async Task<string> GetHTML(Uri URI) {
            HttpClient Client = new HttpClient();

            HttpResponseMessage Response = await Client.GetAsync(URI);

            return await Response.Content.ReadAsStringAsync();
        }
    }
}