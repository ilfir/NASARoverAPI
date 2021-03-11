using System;
using System.Collections.Generic;

namespace NASARoverAPI.Types
{
    public class RoverPhotos
    {
        public List<Photo> photos;

        public class Photo
        {
            public string id { get; set; }
            public string sol { get; set; }
            public Photo camera { get; set; }
            public string img_src { get; set; }
            public string earth_date { get; set; }
            public Rover rover { get; set; }
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
