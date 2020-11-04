using BokuNoGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame.ViewModels
{
    public class GameViewModel
    {
        public Game Game { get; set; }
        public Catalog Catalog { get; set; }
    }
}
