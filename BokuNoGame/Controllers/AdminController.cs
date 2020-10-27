using BokuNoGame.Models;
using BokuNoGame.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame.Controllers
{
    public class AdminController : Controller
    {

        private readonly AppDBContext _context;

        public AdminController(AppDBContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult CreateGame()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateGame(CreateGameViewModel model)
        {
            var lastGame = _context.Games.OrderBy(g => g.Id).LastOrDefault();
            var id = lastGame != null ? lastGame.Id + 1 : 1;
            model.Game = new Models.Game() { Id = id};
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddGameToDB(CreateGameViewModel model)
        {
            if (model.Logo != null)
            {
                byte[] imageData = null;
                // считываем переданный файл в массив байтов
                using (var binaryReader = new BinaryReader(model.Logo.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)model.Logo.Length);
                }
                // установка массива байтов
                model.Game.Logo = imageData;
            }
            await _context.Games.AddAsync(model.Game);
            await _context.SaveChangesAsync();
            return RedirectToAction("Administrate", "Account");
        }
    }
}
