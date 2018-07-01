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
            var deserialisedMovies = (List<MovieModel>)getResponse.Value;
            deserialisedMovies.SequenceEqual(moviesData).Should().BeTrue();
        }

        /// <summary>
        /// Checks that a movie can be retrieved by ID successfully
        /// </summary>
        /// <param name="id"></param>
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

        /// <summary>
        /// Checks that only 1 movie exists per ID
        /// </summary>
        /// <param name="id">the movie ID</param>
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
            var deserializedMovie = (MovieModel)getResponse.Value;
            deserializedMovie.Should().NotBeNull();
        }

        /// <summary>
        /// Checks that if retrieving a non-existent ID, a NotFound status code is responded
        /// </summary>
        /// <param name="id">The movie ID</param>
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
            getResponse.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        /// <summary>
        /// Checks if a new movie can be posted successfully, and that the actor relationship is established
        /// </summary>
        /// <param name="title">Title to go in the movie model</param>
        /// <param name="description">Description to go in the movie model</param>
        /// <param name="actorId">An optional actor that should already exist in the database</param>
        /// <returns></returns>
        [Test]
        [TestCase("The Thing", "John Carpenter horror movie", null)]
        [TestCase("The Thing", "John Carpenter horror movie", 1)]
        public async Task Movies_Post_Successful(string title, string description, int? actorId)
        {
            //Arrange
            var controller = new MoviesController();
            var numMovies = 0;
            using (var dbContext = new MovieDbContext())
            {
                numMovies = dbContext.Movies.Count();
            }

            MovieModel movieModel;
            if (actorId == null)
            {
                movieModel = new MovieModel { Title = title, Description = description };
            }
            else
            {
                movieModel = new MovieModel { Title = title, Description = description, MovieActors = new List<MovieActor> { new MovieActor { ActorId = (int)actorId } } };
            }
            //Act
            var postResponse = await controller.Post(movieModel);
            //Assert
            postResponse.Should().BeOfType(typeof(CreatedResult)); //movie created successfully

            using (var dbContext = new MovieDbContext())
            {
                //verify against the properties of the latest movie in the database
                var movies = dbContext.Movies.Include(m => m.MovieActors).ThenInclude(ma => ma.Actor);
                movies.Count().Should().Be(numMovies + 1);
                var postedMovie = movies.Last();
                postedMovie.Title.Should().Be(title);
                postedMovie.Description.Should().Be(description);
                if (actorId != null)
                {
                    //check that the actor associated with the movie exists
                    postedMovie.MovieActors.Should().ContainSingle();
                    postedMovie.MovieActors.Single().ActorId.Should().Be(actorId);
                }
            }
        }

        /// <summary>
        /// Checks that if a non-existent actor is referenced in a post, this is handled as a Bad Request
        /// </summary>
        /// <param name="title">Title to go in the movie model</param>
        /// <param name="description">Description to go in the movie model</param>
        /// <param name="actorId">Actor ID to reference</param>
        /// <returns></returns>
        [Test]
        [TestCase("The Thing", "John Carpenter horror movie", 100)]
        public async Task Movies_Post_NonExistentActor(string title, string description, int actorId)
        {
            //Arrange
            var controller = new MoviesController();
            MovieModel movieModel = new MovieModel { Title = title, Description = description, MovieActors = new List<MovieActor> { new MovieActor { ActorId = actorId } } };

            //Act
            var postResponse = await controller.Post(movieModel);
            //Assert
            postResponse.Should().BeOfType(typeof(BadRequestObjectResult));
            postResponse.Should().BeOfType(typeof(BadRequestObjectResult));
        }
    }
}
