using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SportWebApplication.Models
{
    public class Competention
    {
        public string Name { get; set; }

        [JsonConverter(typeof(JsonTimeSpanConverter))]
        public TimeSpan Interval { get; set; }
        public bool Start { get; set; }
    }
}
