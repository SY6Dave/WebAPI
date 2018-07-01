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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Text;
using System.Net;
using System;

namespace WebAPI.Tests
{
    [TestFixture]
    public class MoviesTests
    {
        private TestServer testServer;
        private HttpClient testClient;
        private const string URI = "/api/movies";

        [SetUp]
        public void SetUp()
        {
            testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            testClient = testServer.CreateClient();
        }

        [Test]
        public async Task Movies_GetRequest_Returns200Response()
        {
            //Act
            var getResponse = await testClient.GetAsync(URI);

            //Assert
            getResponse.IsSuccessStatusCode.Should().BeTrue();
        }

        /// <summary>
        /// Reads the database and then performs a GET request on the controller
        /// Checks that the returned JSON, when deserialized, is equal to the original data
        /// </summary>
        [Test]
        public async Task Movies_GetRequest_SerializesOK()
        {
            //Arrange
            var moviesData = new List<MovieModel>();

            //Act
            using (var dbContext = new MovieDbContext())
            {
                moviesData = dbContext.Movies.Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                    .ToList();
            }
            var getResponse = await testClient.GetAsync(URI);
            var jsonMovieResponse = getResponse.Content.ReadAsAsync<List<MovieModel>>();
            var deserialisedMovies = jsonMovieResponse.Result;

            //Assert
            getResponse.IsSuccessStatusCode.Should().BeTrue();
            deserialisedMovies.SequenceEqual(moviesData).Should().BeTrue(); //original data and deserialized data should be equal
        }

        /// <summary>
        /// Checks that a single movie can be retrieved by ID successfully
        /// </summary>
        /// <param name="id"></param>
        [Test]
        [TestCase(1)]
        public async Task Movies_GetById_ReturnsOK(int id)
        {
            //Arrange
            MovieModel movie;

            //Act
            var getResponse =  await testClient.GetAsync(String.Format("{0}/{1}", URI, id));
            var jsonMovieResponse = getResponse.Content.ReadAsAsync<MovieModel>();
            movie = jsonMovieResponse.Result;

            //Assert
            getResponse.IsSuccessStatusCode.Should().BeTrue();
            movie.Should().NotBeNull();
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
        public async Task Movies_GetById_ReturnsNotFound(int id)
        {
            //Act
            var getResponse = await testClient.GetAsync(String.Format("{0}/{1}", URI, id));
            //Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
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
        [TestCase("", "", 1)]
        [TestCase("", "", null)]
        public async Task Movies_Post_Successful(string title, string description, int? actorId)
        {
            //Arrange
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
                movieModel = new MovieModel { Title = title, Description = description,
                    MovieActors = new List<MovieActor> { new MovieActor { ActorId = (int)actorId } } };
            }

            //Act
            var postResponse = await testClient.PostAsJsonAsync(URI, movieModel);

            //Assert
            if (title == string.Empty)
            {
                postResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest); //minimum length of a Title is 1 character
            }
            else
            {
                postResponse.StatusCode.Should().Be(HttpStatusCode.Created); //movie created successfully

                using (var dbContext = new MovieDbContext())
                {
                    //verify against the properties of the latest movie in the database
                    var movies = dbContext.Movies.Include(m => m.MovieActors)
                        .ThenInclude(ma => ma.Actor);
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
            MovieModel movieModel = new MovieModel { Title = title, Description = description,
                MovieActors = new List<MovieActor> { new MovieActor { ActorId = actorId } } };

            //Act
            var postResponse = await testClient.PostAsJsonAsync(URI, movieModel);

            //Assert
            postResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Checks that a movie can be updated by ID, and the data is validated
        /// </summary>
        /// <param name="id">The ID of the movie in the database</param>
        /// <param name="title">A new title to give the movie (cannot be empty string)</param>
        /// <param name="description">A new description to give the movie</param>
        /// <returns></returns>
        [Test]
        [TestCase(1, null, null)]
        [TestCase(1, null, "New description")]
        [TestCase(1, "", "")]
        [TestCase(1, "New title", null)]
        [TestCase(1, "New title", "AND new description")]
        [TestCase(100, null, null)]
        [TestCase(100, "", "")]
        public async Task Movies_Put_Successful(int id, string title = null, string description = null)
        {
            //Arrange
            MovieModel updatedMovieModel = new MovieModel { Title = title, Description = description };

            //Act
            var getResponse = await testClient.GetAsync(String.Format("{0}/{1}", URI, id)); //get original movie for comparison
            var putResponse = await testClient.PutAsJsonAsync(String.Format("{0}/{1}", URI, id), updatedMovieModel);

            //Assert
            if (title == String.Empty)
            {
                putResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                return;
            }
            else if (!getResponse.IsSuccessStatusCode)
            {
                putResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
            else
            {
                var originalMovie = getResponse.Content.ReadAsAsync<MovieModel>().Result;
                var updatedMovie = putResponse.Content.ReadAsAsync<MovieModel>().Result;

                if (title == null)
                {
                    updatedMovie.Title.Should().Be(originalMovie.Title);
                }
                else
                {
                    updatedMovie.Title.Should().Be(title);
                }

                if (description == null)
                {
                    updatedMovie.Description.Should().Be(originalMovie.Description);
                }
                else
                {
                    updatedMovie.Description.Should().Be(description);
                }

                putResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Checks that a movie can be irreversably deleted by ID
        /// </summary>
        /// <param name="movieId">The ID of the movie to delete</param>
        /// <returns></returns>
        [Test]
        [TestCase(1)]
        [TestCase(100)]
        public async Task Movies_Delete_RemovedSuccessfully(int movieId)
        {
            //Arrange
            var originalMovie = await testClient.GetAsync(String.Format("{0}/{1}", URI, movieId));

            //Act
            var postResponse = await testClient.DeleteAsync(String.Format("{0}/{1}", URI, movieId));

            //Assert
            postResponse.StatusCode.Should().Be(originalMovie.StatusCode); //if it couldn't be found originally, it can't be deleted
            if(postResponse.IsSuccessStatusCode) //if it deleted, double-check it can't be restored
            {
                var attemptGet = await testClient.GetAsync(String.Format("{0}/{1}", URI, movieId));
                attemptGet.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }
    }
}
