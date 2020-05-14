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
            Informations = new Dictionary<string, string>();
        }

        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("informations")]
        public IDictionary<string, string> Informations { get; set; }
        [JsonProperty("qr_title")]
        public string QRTitle { get; set; }
        [JsonProperty("contact_url")]
        public string ContactUrl { get; set; }
    }
}
