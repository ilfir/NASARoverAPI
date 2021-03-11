using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using NASARoverAPI.Interfaces;
using NASARoverAPI.Types;
using NASARoverAPI.Helpers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NASARoverAPI
{
    public class NasaApiProvider : IApiProvider
    {
        private string apiKey;
        private string apiUrl;
        private string localFolder;
        private List<string> rovers;

        public NasaApiProvider(IOptions<NasaConfiguration> config)
        {
            this.apiKey = config.Value.ApiKey;
            this.apiUrl = config.Value.ApiUrl;
            this.localFolder = config.Value.localFolder;
            this.rovers = config.Value.rovers;
        }

        public void StartDownloadProcess()
        {
            var dateFileContents = DateHelper.GetDatesFromFileAsString("dates.txt");
            var dates = DateHelper.GetFormattedDatesFromString(dateFileContents);
            foreach (var date in dates)
            {
                // TODO: need to figure out what to do with invalid dates.
                if(date.Ticks > 0)
                    DownloadResource(date.ToString("yyyy-MM-dd"));
            }
        }

        public void DownloadResource(string formattedDate)
        {
            foreach (var rover in rovers)
            {
                DownloadRoverDataForDate(rover, formattedDate);
            }
        }

        private void DownloadRoverDataForDate(string rover, string formattedDate)
        {
            var url = apiUrl + $"/mars-photos/api/v1/rovers/{rover}/photos?earth_date={formattedDate}&api_key={apiKey}";            
            var t = Task.Run(() => RestHelper.ProcessGetRequestAsync(url));
            t.Wait();

            RestResponse webResponse = t.Result;
            RoverPhotos roverPhotos = JsonConvert.DeserializeObject<RoverPhotos>(webResponse.data);

            //Kick off writing to disk
            WriteFileToDisk(roverPhotos);

        }

        private void WriteFileToDisk(RoverPhotos roverPhotos)
        {
            //Write files to disk here    
        }
    }
}
