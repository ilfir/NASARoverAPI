using System;
using System.Collections.Generic;

namespace NASARoverAPI.Types
{
    public class RoverPhotosResponse
    {
        public List<Photo> photos { get; set; }

        public class Photo
        {
            public string id { get; set; }
            public string sol { get; set; }
         //   public Camera camera { get; set; }
            public string img_src { get; set; }
            public string earth_date { get; set; }
            public Rover rover { get; set; }
            public bool isDownloaded { get; set; }
        }

        public class Camera
        {
            public string id { get; set; }
            public string name { get; set; }
            public string rover_id { get; set; }
            public string full_name { get; set; }
        }

        public class Rover
        {
            public string id { get; set; }
            public string name { get; set; }
            public string landing_date { get; set; }
            public string launch_date { get; set; }
            public string status { get; set; }
        }
    }
}
