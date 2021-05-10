using BokuNoGame.Extensions;
using BokuNoGame.Filters;
using BokuNoGame.Models;
using BokuNoGame.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame.Controllers
{
    public class GameController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<User> _userManager;

        public GameController(AppDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Search()
        {
            return RedirectToAction("GameList");
        }

        [HttpGet]
        public IActionResult GameList()
        {
            ViewBag.Publishers = new SelectList(_context.Games.Select(g => g.Publisher).Distinct());
            ViewBag.Developers = new SelectList(_context.Games.Select(g => g.Developer).Distinct());
            ViewBag.StartYears = new SelectList(Enumerable.Range(1900, DateTime.Now.Year - 1899));
            ViewBag.EndYears = new SelectList(Enumerable.Range(1900, DateTime.Now.Year - 1899));
            ViewBag.AgeRatings = new SelectList(_context.Games.Select(g => g.AgeRating).Distinct());

            return View();
        }

        [HttpPost]
        public IActionResult GameList(GameListViewModel model)
        {
            var games = _context.Games.AsQueryable();

            ViewBag.Publishers = new SelectList(_context.Games.Select(g => g.Publisher).Distinct());
            ViewBag.Developers = new SelectList(_context.Games.Select(g => g.Developer).Distinct());
            ViewBag.StartYears = new SelectList(Enumerable.Range(1900, DateTime.Now.Year - 1899));
            ViewBag.EndYears = new SelectList(Enumerable.Range(1900, DateTime.Now.Year - 1899));
            ViewBag.AgeRatings = new SelectList(_context.Games.Select(g => g.AgeRating).Distinct());

            if (games.Count() == 1)
                return RedirectToAction("Game", new { gameId = games.First().Id });

            return View(model);
        }

        public async Task<IActionResult> Game(int gameId)
        {
            ViewBag.Catalogs = new SelectList(_context.Catalogs, "Id", "Name");

            var game = _context.Games.Find(gameId);
            var user = await _userManager.GetUserAsync(User);

            Catalog catalog = user != null ? _context.GameSummaries.Include(gs => gs.Catalog).Include(gs => gs.Game)
                .FirstOrDefault(gs => gs.UserId.Equals(user.Id) && gs.GameId == gameId)?.Catalog : null;

            return View(new GameViewModel() { Game = game, Catalog = catalog });
        }

        [Authorize]
        public async Task<IActionResult> AddGameToUserCatalog(int gameId, int catalogId)
        {
            var userId = _userManager.GetUserId(User);
            var game = await _context.Games.FindAsync(gameId);
            var catalog = await _context.Catalogs.FindAsync(catalogId);
            var summary = new GameSummary()
            {
                GameName = game.Name,
                Game = game,
                GameId = game.Id,
                Rate = null,
                Genre = game.Genre,
                GenreWrapper = game.Genre.GetAttribute<DisplayAttribute>().Name,
                UserId = userId,
                Catalog = catalog,
                CatalogId = catalog.Id
            };
            await _context.GameSummaries.AddAsync(summary);
            await _context.SaveChangesAsync();
            return RedirectToAction("Game", new { gameId = game.Id });
        }

        public async Task<IActionResult> UpdateGameInUserCatalog(int gameId, int catalogId)
        {
            var userId = _userManager.GetUserId(User);
            var catalog = await _context.Catalogs.FindAsync(catalogId);
            var summary = _context.GameSummaries.First(gs => gs.GameId.Equals(gameId) && gs.UserId.Equals(userId));
            summary.Catalog = catalog;
            summary.CatalogId = catalog.Id;
            await _context.SaveChangesAsync();
            return RedirectToAction("Game", new { gameId });
        }

        public IActionResult DeleteGameFromUserCatalog(int gameSummaryId)
        {
            var summary = _context.GameSummaries.Find(gameSummaryId);
            if (summary != null)
            {
                _context.GameSummaries.Remove(summary);
                _context.SaveChanges();
                return Ok();
            }
            return RedirectToAction("Profile", "Account");
        }

        public async Task<IActionResult> CreateGameReview(int gameId, string text)
        {
            var review = new Review
            {
                Text = text,
                GameId = gameId,
                UserId = _userManager.GetUserId(User),
                Date = DateTime.Now,
                IsApproved = false
            };
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return RedirectToAction("Game", new { gameId });
        }
    }
}
