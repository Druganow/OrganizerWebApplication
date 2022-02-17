using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportWebApplication.Models
{
    public class Competention
    {
        public string Name { get; set; }
        public TimeSpan Interval { get; set; }
        public bool Start { get; set; }
    }
}
