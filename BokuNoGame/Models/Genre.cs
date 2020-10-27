﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame.Models
{
    public enum Genre
    {
        [Display(Name = "Экшен")]
        Action,
        [Display(Name = "Симулятор")]
        Simulation,
        [Display(Name = "Стратегия")]
        Strategy,
        [Display(Name = "РПГ")]
        RPG,
        [Display(Name = "Головоломка")]
        Puzzle,
        [Display(Name = "Аркада")]
        Arcade
    }
}
