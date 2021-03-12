using System;
using System.Collections.Generic;

namespace NASARoverAPI.Interfaces
{
    // This may not be needed
    public interface IApiProvider
    {
        void DownloadResource(string resourceName);

        void StartDownloadProcess();
    }
}
