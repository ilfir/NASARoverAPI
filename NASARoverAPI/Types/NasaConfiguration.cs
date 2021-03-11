using System;
using System.Collections.Generic;

namespace NASARoverAPI.Types
{
    public class NasaConfiguration
    {
        public string ApiKey { get; set; }

        public string ApiUrl { get; set; }

        public string localFolder { get; set; }

        public List<string> rovers {get; set;}
    }
}
