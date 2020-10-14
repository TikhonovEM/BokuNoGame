using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame.Models
{
    public class Catalog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<GameSummary> GameSummaries {get; set;}
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
