using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public class HomePageConfig
    {
        public HomePageConfig()
        {
            Lines = new List<string>();
        }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
        [JsonProperty("lines")]
        public IList<string> Lines { get; set; }
    }
}
