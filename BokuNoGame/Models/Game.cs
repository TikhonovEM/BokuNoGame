using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public Genre Genre { get; set; }
        public double Rating { get; set; }
        public string Publisher { get; set; }
        public string Developer { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string AgeRating { get; set; }

    }
}
