using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportWebApplication.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public int Age { get; set; }
        public int Sex { get; set; }
        public int StartTime { get; set; }
        public int FinishTime { get; set; }
        public int ResultTime { get; set; }
        public int RandNumber { get; set; }
    }
}
