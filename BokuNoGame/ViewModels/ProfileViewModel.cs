﻿using BokuNoGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame.ViewModels
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public IQueryable<GameSummary> GameSummaries { get; set; }
    }
}
