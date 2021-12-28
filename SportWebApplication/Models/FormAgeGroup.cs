using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportWebApplication.Models
{
    public class FormAgeGroup
    {
        public int Yahr1 { get; set; } 
        public int Yahr2 { get; set; } 
        public int LapsM { get; set; }
        public int LapsF { get; set; }
        public List<AgeGroup> AG{ get; set; }
    }
}
