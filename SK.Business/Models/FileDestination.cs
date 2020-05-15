using elFinder.NetCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    public class FileDestination
    {
        [JsonProperty("ef_hash")]
        public string EFHash { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("relative_path")]
        public string RelativePath { get; set; }
    }

    public class FileDestinationMetadata
    {
        public RootVolume RootVolume { get; set; }
        public string SourceUrl { get; set; }
        public string RootPath { get; set; }

        public FileDestinationMetadata(RootVolume rootVolume = null, string sourceUrl = null,
            string rootPath = null)
        {
            RootVolume = rootVolume;
            SourceUrl = sourceUrl;
            RootPath = rootPath;
        }
    }
}
