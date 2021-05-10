using BokuNoGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame.Filters
{
    public class FilterPanel
    {
        public string Name { get; set; }
        public Genre Genre { get; set; }
        public string Publisher { get; set; }
        public string Developer { get; set; }
        public int? ReleaseYearStart { get; set; }
        public int? ReleaseYearEnd { get; set; }
        public double? Rating { get; set; }
        public string AgeRating { get; set; }
    }
}
