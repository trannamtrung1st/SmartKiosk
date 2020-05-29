using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public class ContactConfig
    {
        public ContactConfig()
        {
            Information = new Dictionary<string, string>();
        }

        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("information")]
        public IDictionary<string, string> Information { get; set; }
        [JsonProperty("qr_title")]
        public string QRTitle { get; set; }
        [JsonProperty("contact_url")]
        public string ContactUrl { get; set; }
    }
}
