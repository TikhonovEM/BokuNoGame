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
using Microsoft.AspNetCore.Identity;

namespace BokuNoGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _context;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, AppDBContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateNews(string title, string text, string reference, bool isLocal)
        {
            var user = await _userManager.GetUserAsync(User);
            var news = new News
            {
                AuthorId = user.Id,
                Title = title,
                Text = text,
                Reference = reference,
                IsLocal = isLocal
            };
            await _context.News.AddAsync(news);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
