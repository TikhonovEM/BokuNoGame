using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame.Models
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<GameSummary> GameSummaries { get; set; }

        public IQueryable<Game> GetTopMostPopularGames(int top)
        {
            return Games
                .OrderByDescending(g => GameSummaries.Where(gs => gs.GameId == g.Id && gs.CatalogId == 2).Count())
                .Take(top);
        }

        public IQueryable<GameSummary> GetGameSummaries(string userId)
        {
            return GameSummaries.Where(gs => gs.UserId.Equals(userId));
        }
    }
}
