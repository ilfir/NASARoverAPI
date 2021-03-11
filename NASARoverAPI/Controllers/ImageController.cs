using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NASARoverAPI.Types;
using System.Text.Json;

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
        public string Index()
        {
            return "Nothing happens here";         
        }

        [HttpGet("DownloadFiles")]
        public ApiProviderResult DownloadFiles()
        {

            NasaApiProvider provider = new NasaApiProvider(nasaConfiguration);
            ApiProviderResult result = provider.DownloadFiles("dates.txt");
            return result;
        }
    }
}