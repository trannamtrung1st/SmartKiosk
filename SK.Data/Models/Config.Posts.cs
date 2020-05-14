using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public class PostsConfig
    {
        [JsonProperty("mode")]
        public PostsConfigMode Mode { get; set; }
        [JsonProperty("playlist")]
        public List<PostPlaylistItems> Playlist { get; set; }

        public PostsConfig()
        {
            Playlist = new List<PostPlaylistItems>();
        }

    }

    public class PostPlaylistItems
    {
        [JsonProperty("pos")]
        public int? Position { get; set; }
        [JsonProperty("post_id")]
        public int PostId { get; set; }
    }
}
