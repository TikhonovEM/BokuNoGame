using BokuNoGame.Filters;
using BokuNoGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame.ViewModels
{
    public class GameListViewModel
    {
        public FilterPanel Filter { get; set; }
        public List<Game> Games { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
