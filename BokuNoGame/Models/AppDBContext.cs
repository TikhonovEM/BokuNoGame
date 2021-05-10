using BokuNoGame.Filters;
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
        public DbSet<IntegrationInfo> IntegrationInfos { get; set; }
        public DbSet<Review> Reviews { get; set; }

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

        public IQueryable<Review> GetGameReviews(int gameId)
        {
            return Reviews.Where(r => r.GameId == gameId);
        }

        public IQueryable<Game> GetFilteredGameList(FilterPanel filter)
        {
            var games = Games.AsQueryable();
            if (filter == null)
                return games;

            if (!string.IsNullOrEmpty(filter.Name))
                games = games.Where(g => g.Name.Contains(filter.Name));

            if (filter.Genre != Genre.Default)
                games = games.Where(g => g.Genre == filter.Genre);

            if (filter.Rating.HasValue)
                games.Where(g => g.Rating >= filter.Rating);
            
            if (!string.IsNullOrEmpty(filter.Publisher))
                games = games.Where(g => g.Publisher.Equals(filter.Publisher));

            if (!string.IsNullOrEmpty(filter.Developer))
                games = games.Where(g => g.Developer.Equals(filter.Developer));

            if (filter.ReleaseYearStart.HasValue && filter.ReleaseYearEnd.HasValue)
                games = games.Where(g => g.ReleaseDate.Year >= filter.ReleaseYearStart && g.ReleaseDate.Year <= filter.ReleaseYearEnd);
            else if (filter.ReleaseYearStart.HasValue && !filter.ReleaseYearEnd.HasValue)
                games = games.Where(g => g.ReleaseDate.Year >= filter.ReleaseYearStart);
            else if (!filter.ReleaseYearStart.HasValue && filter.ReleaseYearEnd.HasValue)
                games = games.Where(g => g.ReleaseDate.Year <= filter.ReleaseYearEnd);

            if (filter.Rating.HasValue)
                games = games.Where(g => g.Rating >= filter.Rating);

            if (!string.IsNullOrEmpty(filter.AgeRating))
                games = games.Where(g => g.AgeRating.Equals(filter.AgeRating));

            return games;
        }
    }
}
