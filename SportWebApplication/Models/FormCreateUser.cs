using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportWebApplication.Models
{
    public class FormCreateUser
    {
        public Sportsman sportsman { get; set; }
        public List<Sportsman> userList { get; set; }
    }
}
