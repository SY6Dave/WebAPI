using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using WebAPI.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WebAPI.Tests
{
    [TestFixture]
    public class MovieDbContextTests
    {
        [Test]
        public void MovieDbContext_Seeding_HasData()
        {
            //Arrange
            using (var db = new MovieDbContext())
            {
                //Act
                db.Database.EnsureDeleted();
                db.Database.Migrate();
                db.Seed();
                var movies = db.Movies.Include(m => m.MovieActors).ToList();
                var actors = db.Actors.Include(a => a.MovieActors).ToList();

                //Assert
                movies.Should().NotBeNullOrEmpty();
                actors.Should().NotBeNullOrEmpty();
            }                
        }
    }
}
