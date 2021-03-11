using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NASARoverAPI.Types;

namespace NASARoverAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IOptions<NasaConfiguration> nasaConfiguration;

        public ImageController(IOptions<NasaConfiguration> nasaConfiguration)
        {
            this.nasaConfiguration = nasaConfiguration;
        }
                
        [HttpGet("")]
        public IEnumerable<string> Index()
        {
            var dateFileContents = DateHelper.GetDatesFromFileAsString("dates.txt");
            var dates = DateHelper.GetFormattedDatesFromString(dateFileContents);

            NasaApiProvider provider = new NasaApiProvider(nasaConfiguration);
            

            var resp = new List<string>();

            foreach (var date in dates)
            {
                resp.Add(date.ToShortDateString());
            }


            return resp;
        }

        [HttpGet("DownloadFiles")]
        public ActionResult DownloadFiles()
        {

            NasaApiProvider provider = new NasaApiProvider(nasaConfiguration);

            provider.StartDownloadProcess();
            // DownloadFiles()


            var path = @"c:\FileDownload.csv";
            //return File((path, "text/plain", sourceLocaion);
            return null;
        }
    }
}