using System;
using System.Net.Http;
using NASARoverAPI.Types;

namespace NASARoverAPI.Helpers
{
    public static class RestHelper
    {
        public static async System.Threading.Tasks.Task<RestResponse> ProcessGetRequestAsync(string apiUrl)
        {

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = await client.GetAsync(apiUrl))
            using (HttpContent content = res.Content)
            {
                string data = await content.ReadAsStringAsync();
                if (data != null)
                {
                    return new RestResponse() { success = true, data = data};
                }
            }
            return new RestResponse() { success = false };
        }
        
    }
}
