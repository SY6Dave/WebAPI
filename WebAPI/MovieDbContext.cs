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

        /// <summary>
        /// Seed the database with some test data
        /// </summary>
        public void Seed()
        {
            //Set-up some movies
            MovieModel movieTheyLive = new MovieModel { Title = "They Live", Description = "Science fiction thriller, directed by John Carpenter", MovieActors = new List<MovieActor>() };
            MovieModel movieTaxiDriver = new MovieModel { Title = "Taxi Driver", Description = "Martin Scorsese, neo-noir classic", MovieActors = new List<MovieActor>() };
            MovieModel movieCapeFear = new MovieModel { Title = "Cape Fear", Description = "Creepy boat scene, parodied by The Simpsons", MovieActors = new List<MovieActor>() };
            MovieModel movie48Hrs = new MovieModel { Title = "48 Hrs", Description = "80s cop film", MovieActors = new List<MovieActor>() };

            //Set-up some actors
            ActorModel actorRoddyPiper = new ActorModel { FirstName = "'Rowdy' Roddy", Surname = "Piper", MovieActors = new List<MovieActor>() };
            ActorModel actorKeithDavid = new ActorModel { FirstName = "Keith", Surname = "David", MovieActors = new List<MovieActor>() };
            ActorModel actorRobertDeNiro = new ActorModel { FirstName = "Robert", Surname = "De Niro", MovieActors = new List<MovieActor>() };
            ActorModel actorHarveyKeitel = new ActorModel { FirstName = "Harvey", Surname = "Keitel", MovieActors = new List<MovieActor>() };
            ActorModel actorNickNolte = new ActorModel { FirstName = "Nick", Surname = "Nolte", MovieActors = new List<MovieActor>() };
            ActorModel actorEddieMurphy = new ActorModel { FirstName = "Eddie", Surname = "Murphy", MovieActors = new List<MovieActor>() };

            //Set-up the relationships
            MovieActor theyLiveRoddyPiper = new MovieActor { Movie = movieTheyLive, Actor = actorRoddyPiper };
            MovieActor theyLiveKeithDavid = new MovieActor { Movie = movieTheyLive, Actor = actorKeithDavid };
            MovieActor taxiDriverDeNiro = new MovieActor { Movie = movieTaxiDriver, Actor = actorRobertDeNiro };
            MovieActor taxiDriverKeitel = new MovieActor { Movie = movieTaxiDriver, Actor = actorHarveyKeitel };
            MovieActor capeFearDeNiro = new MovieActor { Movie = movieCapeFear, Actor = actorRobertDeNiro };
            MovieActor capeFearNolte = new MovieActor { Movie = movieCapeFear, Actor = actorNickNolte };
            MovieActor hrsNolte = new MovieActor { Movie = movie48Hrs, Actor = actorNickNolte };
            MovieActor hrsMurphy = new MovieActor { Movie = movie48Hrs, Actor = actorEddieMurphy };

            //Add the relationships
            movieTheyLive.MovieActors.Add(theyLiveRoddyPiper);
            movieTheyLive.MovieActors.Add(theyLiveKeithDavid);
            movieTaxiDriver.MovieActors.Add(taxiDriverDeNiro);
            movieTaxiDriver.MovieActors.Add(taxiDriverKeitel);
            movieCapeFear.MovieActors.Add(capeFearDeNiro);
            movieCapeFear.MovieActors.Add(capeFearNolte);
            movie48Hrs.MovieActors.Add(hrsMurphy);
            movie48Hrs.MovieActors.Add(hrsNolte);

            actorRoddyPiper.MovieActors.Add(theyLiveRoddyPiper);
            actorKeithDavid.MovieActors.Add(theyLiveKeithDavid);
            actorRobertDeNiro.MovieActors.Add(taxiDriverDeNiro);
            actorRobertDeNiro.MovieActors.Add(capeFearDeNiro);
            actorHarveyKeitel.MovieActors.Add(taxiDriverKeitel);
            actorNickNolte.MovieActors.Add(capeFearNolte);
            actorNickNolte.MovieActors.Add(hrsNolte);
            actorEddieMurphy.MovieActors.Add(hrsMurphy);

            Movies.AddRange(movieTheyLive, movieTaxiDriver, movieCapeFear, movie48Hrs);
            Actors.AddRange(actorRoddyPiper, actorKeithDavid, actorRobertDeNiro, actorHarveyKeitel, actorNickNolte, actorEddieMurphy);
            SaveChanges();
        }
    }
}