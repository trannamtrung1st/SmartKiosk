using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    public class QueryResult<T>
    {
        [JsonProperty("results")]
        public IEnumerable<T> Results { get; set; }
        [JsonProperty("single_result", NullValueHandling = NullValueHandling.Ignore)]
        public T SingleResult { get; set; }
        [JsonProperty("total_count", NullValueHandling = NullValueHandling.Ignore)]
        public int? TotalCount { get; set; }
    }
}
