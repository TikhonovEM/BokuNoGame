using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BokuNoGame.Models;
using Microsoft.AspNetCore.Authorization;
using BokuNoGame.ViewModels;

namespace BokuNoGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _context;

        public HomeController(ILogger<HomeController> logger, AppDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GameList(string likeName)
        {
            var games = _context.Games.Where(g => g.Name.Contains(likeName)).ToList();
            if (games.Count == 1)
                return RedirectToAction("Game", new { gameId = games[0].Id });
            return View(new GameListViewModel { Games = games });
        }

        public IActionResult Game(int gameId)
        {
            var game = _context.Games.Find(gameId);
            return View(game);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
