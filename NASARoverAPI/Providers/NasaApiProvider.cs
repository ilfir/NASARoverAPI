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
    public class NasaApiProvider// : IApiProvider
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

        public ApiProviderResult DownloadFiles(string inputFile)
        {
            ApiProviderResult result;

            // Read in the dates from input file
            var dateFileContents = DateHelper.GetDatesFromFileAsString(inputFile);
            var dates = DateHelper.GetFormattedDatesFromString(dateFileContents);

            if (dates.Count > 0)
            {
                result = new ApiProviderResult();
                result.data = new List<ApiProviderResult.ImageData>();
                List<Task> tasks = new List<Task>();
                // Process all the dates from input file and keep track of processing for each date
                foreach (var date in dates)
                {
                    // TODO: need to figure out what to do with invalid dates.
                    if (date.Ticks > 0)
                       tasks.Add(Task.Run(() => DownloadFilesForDate(date.ToString("yyyy-MM-dd"))));
                }
                Task.WaitAll(tasks.ToArray());
                                
                foreach (Task<Dictionary<string, List<RoverPhotosResponse>>> task in tasks)
                {
                    var taskResult = task.Result;
                    var imageData = new ApiProviderResult.ImageData();
                    foreach (var roverImageData in taskResult)
                    {
                        var date = roverImageData.Key;
                        foreach (var roverImage in roverImageData.Value)
                        {
                            roverImage.photos.ForEach(delegate (RoverPhotosResponse.Photo photo)
                            {
                                var imageData = new ApiProviderResult.ImageData()
                                {
                                    image = photo.img_src,
                                    roverName = photo.rover.name,
                                    date = date,
                                    isDownloaded = photo.isDownloaded                                
                                };
                                result.data.Add(imageData);
                            });
                        }
                    }                    
                }
                result.success = true;
            }
            else
            {
                result = new ApiProviderResult(){ message = "Unable to read dates for input." };
            }

            return result;
        }

        public Dictionary<string, List<RoverPhotosResponse>> DownloadFilesForDate(string formattedDate)
        {
            
            List<RoverPhotosResponse> roverData = new List<RoverPhotosResponse>();
            foreach (var rover in rovers)
            {
                roverData.Add(DownloadRoverDataForDate(rover, formattedDate));
            }
            Dictionary<string, List<RoverPhotosResponse>> dateData = new Dictionary<string, List<RoverPhotosResponse>>();
            dateData.Add(formattedDate, roverData);
            return dateData;
        }

        private RoverPhotosResponse DownloadRoverDataForDate(string rover, string formattedDate)
        {
            var url = apiUrl + $"/mars-photos/api/v1/rovers/{rover}/photos?earth_date={formattedDate}&api_key={apiKey}";            
            var t = Task.Run(() => RestHelper.ProcessGetRequestAsync(url));
            t.Wait();

            RestResponse webResponse = t.Result;
            RoverPhotosResponse roverPhotos = JsonConvert.DeserializeObject<RoverPhotosResponse>(webResponse.data);

            // Kick off writing to disk
            WriteFileToDisk(roverPhotos);

            return roverPhotos;
        }

        private void WriteFileToDisk(RoverPhotosResponse roverPhotos)
        {
            //Write files to disk here    
        }
    }
}
