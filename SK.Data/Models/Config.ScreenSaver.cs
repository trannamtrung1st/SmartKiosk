using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public class ScreenSaverPlaylist
    {

        [JsonProperty("medias")]
        public List<PlaylistMedia> Medias { get; set; }
        [JsonProperty("mode")]
        public ScreenSaverPlaylistMode Mode { get; set; }

        public ScreenSaverPlaylist()
        {
            Medias = new List<PlaylistMedia>();
            Mode = ScreenSaverPlaylistMode.InOrder;
        }
    }

    public class PlaylistMedia
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("type")]
        public PlaylistMediaType Type { get; set; }
        [JsonProperty("pos")]
        public int? Position { get; set; }

        public override bool Equals(object obj)
        {
            return obj is PlaylistMedia media &&
                   Id == media.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
