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
using System.Linq;

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
                result.data = new List<ImageData>();
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
                    var imageData = new ImageData();
                    foreach (var roverImageData in taskResult)
                    {
                        var date = roverImageData.Key;
                        foreach (var roverImage in roverImageData.Value)
                        {
                            roverImage.photos.ForEach(delegate (RoverPhotosResponse.Photo photo)
                            {
                                var imageData = new ImageData()
                                {
                                    imageSource = photo.img_src,
                                    filename = Path.GetFileName(photo.img_src),
                                    localPath = $"{localFolder}{date}/{photo.rover.name}/{Path.GetFileName(photo.img_src)}" ,
                                    roverName = photo.rover.name,
                                    date = date,
                                    isDownloaded = photo.isDownloaded,
                                    isExists = photo.isExists
                                };
                                result.data.Add(imageData);
                            });
                        }
                    }
                }

                // Save metadata for more performant viewing process later.
                SaveMetadata(result);

                result.success = true;
                result.message = $"Images found on NASA site: {result.data.Count}. Number of images downloaded: {result.data.Where(w => w.isDownloaded).Count()}. ";
                result.message += $"Number of images previously downloaded: {result.data.Where(w => w.isExists).Count()}.";
            }
            else
            {
                result = new ApiProviderResult() { message = "Unable to read dates for input." };
            }

            return result;
        }

        public Dictionary<string, List<RoverPhotosResponse>> DownloadFilesForDate(string formattedDate)
        {

            List<RoverPhotosResponse> roverData = new List<RoverPhotosResponse>();
            foreach (var rover in rovers)
            {
                var roverPhotos = DownloadRoverDataForDate(rover, formattedDate);
                roverData.Add(roverPhotos);

                // Kick off writing to disk
                WriteFilesToDisk(roverPhotos);
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
            return roverPhotos;
        }

        private void WriteFilesToDisk(RoverPhotosResponse roverPhotos)
        {
            // Setup directory structure
            CreateMissingDirectories(roverPhotos);

            using (var client = new WebClient())
            {                
                foreach (var roverPhoto in roverPhotos.photos)
                {
                    var roverFileFolder = GetRoverPhotoFolderName(roverPhoto);
                    var downloadUrl = roverPhoto.img_src + "?api_key=" + apiKey;
                    var localFileName = System.IO.Path.GetFileName(roverPhoto.img_src);
                    var localFileFullPath = roverFileFolder + localFileName;
                    if (!File.Exists(localFileFullPath))
                    {
                        client.DownloadFile(downloadUrl, localFileFullPath);
                        roverPhoto.isDownloaded = true;
                    }
                    else {
                        roverPhoto.isExists = true;
                    }
                }
            }
        }

        private void CreateMissingDirectories(RoverPhotosResponse roverPhotos)
        {
            foreach (var roverPhoto in roverPhotos.photos)
            {
                var roverFileFolder = GetRoverPhotoFolderName(roverPhoto);
                if (!System.IO.Directory.Exists(roverFileFolder))
                    System.IO.Directory.CreateDirectory(roverFileFolder);
            }
        }

        private string GetRoverPhotoFolderName(RoverPhotosResponse.Photo roverPhoto)
        {
            return localFolder + $"{roverPhoto.earth_date}/{roverPhoto.rover.name}/";
        }

        private void SaveMetadata(ApiProviderResult imageData)
        {
            var metadataFolder = localFolder + "/metadata/";
            if (!System.IO.Directory.Exists(metadataFolder))
                System.IO.Directory.CreateDirectory(metadataFolder);

            var metadataFileFullPath = metadataFolder + "metadata.json";
            if (File.Exists(metadataFileFullPath))
                File.Delete(metadataFileFullPath);

            using (TextWriter createStream = new StreamWriter(metadataFileFullPath))
                (new JsonSerializer()).Serialize(createStream, imageData);
        }

        public ApiProviderResult GetMetadata()
        {
            var metadataFolder = localFolder + "/metadata/";
            var metadataFileFullPath = metadataFolder + "metadata.json";
            if (File.Exists(metadataFileFullPath))
            {
                string json = File.ReadAllText(metadataFileFullPath);
                ApiProviderResult metadata = JsonConvert.DeserializeObject<ApiProviderResult>(json);
                return metadata;
            }
            return null;
        }

    }
}
