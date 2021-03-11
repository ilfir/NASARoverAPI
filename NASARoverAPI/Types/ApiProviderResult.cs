using System;
using System.Collections.Generic;

namespace NASARoverAPI.Types
{
    public class ApiProviderResult
    {
        public bool success { get; set; } = false;

        public string message { get; set; }

        public List<ImageData> data { get; set; }

        public class ImageData
        {            
            public string date { get; set; }
            public string roverName { get; set; }
            public bool isDownloaded { get; set; }
            public string image { get; set; }
        }

    }
}
