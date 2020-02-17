using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIPractice.Models
{
    public class Deck
    {
        public string deckid { get; set; }
        public List<Card> currentCards { get; set; }
    }
}
