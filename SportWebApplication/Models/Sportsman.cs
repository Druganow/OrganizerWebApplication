using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportWebApplication.Models
{
    public class Sportsman
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public int Age { get; set; }
        public int Sex { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan FinishTime { get; set; }
        public TimeSpan ResultTime { get; set; }
        public int RandNumber { get; set; }
    }
}
