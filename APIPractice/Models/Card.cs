using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIPractice.Models
{
    public class Card
    {
        public string deck_id { get; set; }
        public string value { get; set; }
        public string image { get; set; }
        public string suit { get; set; }
        public string code { get; set; }
        
        public bool isChecked { get; set; }



    }
}
