using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NASARoverAPI.Models;
using NASARoverAPI.Types;
using Newtonsoft.Json;

namespace NASARoverAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        private HttpClient HttpClient
        {
            get
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri($"https://{Request.Host.Value}/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return client;
            }
        }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> DownloadFilesAsync()
        {
            using (HttpClient)
            {
                HttpResponseMessage Res = await HttpClient.GetAsync("api/image/downloadfiles");
                var data = await Res.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<ApiProviderResultShort>(data);                

                ViewData["Success"] = apiResult.success;
                ViewData["Message"] = apiResult.message;

                return View();
            }
        }


        public async Task<IActionResult> ViewFilesAsync()
        {
            using (HttpClient)
            {
                HttpResponseMessage Res = await HttpClient.GetAsync("api/image/getmetadata");
                var data = await Res.Content.ReadAsStringAsync();
                var metadata = JsonConvert.DeserializeObject<ApiProviderResult>(data);
                ViewData["Images"] = metadata.data;
                return View();
            }
        }

        public ActionResult GetImage(string location)
        {
            if (System.IO.File.Exists(location))
                return base.File(System.IO.File.ReadAllBytes(location), "image/jpeg");
            else
                return base.File(System.IO.File.ReadAllBytes("notfound.jpg"), "image/jpeg");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
