using System;
using System.Net.Http;
using NASARoverAPI.Types;

namespace NASARoverAPI.Helpers
{
    public static class RestHelper
    {
        /// <summary>
        /// Helper for making GET requests.
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<RestResponse> ProcessGetRequestAsync(string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                // Hardcoding due to certificate issues when running in docker.
                // These are local connections so using insecure port is OK for now.
                if (!apiUrl.StartsWith("http"))
                    client.BaseAddress = new Uri($"http://localhost:80/");

                using (HttpResponseMessage res = await client.GetAsync(apiUrl))
                using (HttpContent content = res.Content)
                {
                    string data = await content.ReadAsStringAsync();
                    if (data != null)
                    {
                        return new RestResponse() { success = true, data = data };
                    }
                }
            }
            return new RestResponse() { success = false };
        }        
    }
}
