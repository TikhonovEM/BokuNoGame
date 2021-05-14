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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CreateGame()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CreateGame(CreateGameViewModel model)
        {
            model.Game = new Game();
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

        [Authorize(Roles = "Admin")]
        public IActionResult NewReviews()
        {
            return View();
        }

        public async Task<IActionResult> ApproveReview(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            review.IsApproved = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("NewReviews");
        }

        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return RedirectToAction("NewReviews");
        }
    }
}
