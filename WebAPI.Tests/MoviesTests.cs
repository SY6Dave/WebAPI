using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using WebAPI.Controllers;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace WebAPI.Tests
{
    [TestFixture]
    public class MoviesTests
    {
        [Test]
        public void Movies_GetRequest_Returns200Response()
        {
            //Arrange
            var controller = new MoviesController();
            //Act
            var getResponse = controller.Get();
            //Assert
            getResponse.Should().BeOfType(typeof(OkObjectResult));
        }

        /// <summary>
        /// Reads the database and then performs a GET request on the controller
        /// Checks that the returned JSON, when deserialized, is equal to the original data
        /// </summary>
        [Test]
        public void Movies_GetRequest_SerializesOK()
        {
            //Arrange
            var controller = new MoviesController();
            var moviesData = new List<MovieModel>();
            //Act
            using (var dbContext = new MovieDbContext())
            {
                moviesData = dbContext.Movies.Include(m => m.MovieActors).ThenInclude(ma => ma.Actor).ToList();
            }
            var getResponse = controller.Get() as OkObjectResult;
            //Assert
            getResponse.Should().NotBeNull();
            var json = (string)getResponse.Value;
            var deserialisedMovies = JsonConvert.DeserializeObject<List<MovieModel>>(json);
            deserialisedMovies.SequenceEqual(moviesData).Should().BeTrue();
        }

        [Test]
        [TestCase(1)]
        public void Movies_GetById_ReturnsOK(int id)
        {
            //Arrange
            var controller = new MoviesController();
            //Act
            var getResponse = controller.Get(id);
            //Assert
            getResponse.Should().BeOfType(typeof(OkObjectResult));
        }

        [Test]
        [TestCase(1)]
        public void Movies_GetById_ReturnsSingle(int id)
        {
            //Arrange
            var controller = new MoviesController();
            //Act
            var getResponse = controller.Get(id) as OkObjectResult;
            //Assert
            getResponse.Should().NotBeNull();
            var json = (string)getResponse.Value;
            var deserializedMovie = JsonConvert.DeserializeObject<MovieModel>(json);
            deserializedMovie.Should().NotBeNull();
        }

        [Test]
        [TestCase(99)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(int.MaxValue)]
        public void Movies_GetById_ReturnsNotFound(int id)
        {
            //Arrange
            var controller = new MoviesController();
            //Act
            var getResponse = controller.Get(id);
            //Assert
            getResponse.Should().BeOfType(typeof(NotFoundResult));
        }
    }
}