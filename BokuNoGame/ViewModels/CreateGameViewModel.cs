using BokuNoGame.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame.ViewModels
{
    public class CreateGameViewModel
    {
        public Game Game { get; set; }
        [Display(Name = "Логотип игры")]
        public IFormFile Logo { get; set; }
    }
}
