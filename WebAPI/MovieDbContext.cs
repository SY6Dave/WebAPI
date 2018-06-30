using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI
{
    public class MovieDbContext : DbContext
    {
        public DbSet<MovieModel> Movies { get; set; }
        public DbSet<ActorModel> Actors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=movies.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MovieActor>()
            .HasKey(t => new { t.MovieId, t.ActorId });
        }
    }
}