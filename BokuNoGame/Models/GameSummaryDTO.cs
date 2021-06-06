using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame.Models
{
    public class GameSummaryDTO
    {
        public string GameName { get; set; }
        public int? Rate { get; set; }
        public Genre Genre { get; set; }
        public int GameId { get; set; }
        public int CatalogId { get; set; }
    }
}
