
using Microsoft.EntityFrameworkCore;
using QuakeLogger.Data.Context.Maps;
using QuakeLogger.Models;

namespace QuakeLogger.Data.Context
{
    public class QuakeLoggerContext : DbContext
    {
        public QuakeLoggerContext(DbContextOptions options) : base(options) { }


        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("InMemoryProvider");
        }        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new GameMap());
            modelBuilder.ApplyConfiguration(new PlayerMap());
            base.OnModelCreating(modelBuilder);
        }

    }
}