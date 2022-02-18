using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SportWebApplication.Models
{
    public class Sportsman
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public int Age { get; set; }
        public int Sex { get; set; }
        [JsonConverter(typeof(JsonTimeSpanConverter))]
        public TimeSpan StartTime { get; set; }
        [JsonConverter(typeof(JsonTimeSpanConverter))]
        public TimeSpan FinishTime { get; set; }
        [JsonConverter(typeof(JsonTimeSpanConverter))]
        public TimeSpan ResultTime { get; set; }
        public int RandNumber { get; set; }
    }
}
