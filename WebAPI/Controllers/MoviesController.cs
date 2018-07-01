using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using Microsoft.Extensions.Logging;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ILogger<MoviesController> _log;

        public MoviesController(ILogger<MoviesController> log)
        {
            _log = log;
        }


        /// <summary>
        /// Get all the movies in the database
        /// </summary>
        /// <returns>A list of all movies</returns>
        /// <response code="200">Returns the list of movies</response>
        // GET api/movies
        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult Get()
        {
            using (var dbContext = new MovieDbContext())
            {
                var movies = dbContext.Movies.Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor);

                return Ok(movies.ToList());
            }
        }

        /// <summary>
        /// Get a specific movie by ID
        /// </summary>
        /// <param name="id">The ID of the movie to get</param>
        /// <returns>The movie requested</returns>
        /// <response code="200">Returns the movie requested</response>
        /// <response code="404">If a single movie with the requested ID cannot be found</response>
        // GET api/movies/1
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult Get(int id)
        {
            using (var dbContext = new MovieDbContext())
            {
                try
                {
                    var movie = dbContext.Movies.Where(m => m.MovieId == id).Include(m => m.MovieActors)
                   .ThenInclude(ma => ma.Actor)
                   .Single();

                    return Ok(movie);
                }
                catch (InvalidOperationException ex)
                {
                    _log.LogError(ex.GetType().FullName + " :: " + ex.Message);
                    return NotFound("Unable to find a single movie with the requested ID");
                }
            }
        }

        /// <summary>
        /// Create a new movie
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/movies
        ///     {
	    ///         "Title":"Jurassic Park",
	    ///         "Description":"Dinosaurs",
	    ///         "MovieActors":
	    ///          [
		///             {
		///             	"ActorId": 5
        ///             }
	    ///          ]
        ///     }
        /// </remarks>
        /// <param name="value">A MovieModel object which can also contain references to existing actors by ID</param>
        /// <returns>Returns the newly created movie</returns>
        /// <response code="201">The movie was created successfully</response>
        /// <response code="400">If a single actor with one of the referenced IDs cannot be found</response>
        // POST api/movies
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody] MovieModel value)
        {
            if (value == null)
                return BadRequest();

            using (var dbContext = new MovieDbContext())
            {
                if (value.MovieActors != null)
                {
                    //Add the relationship between Movie and Actor
                    foreach (var ma in value.MovieActors)
                    {
                        try
                        {
                            ma.Actor = dbContext.Actors.Where(a => a.ActorId == ma.ActorId).Single();
                        }
                        catch(InvalidOperationException ex)
                        {
                            _log.LogError(ex.GetType().FullName + " :: " + ex.Message);
                            return BadRequest("Unable to find actor ID in database");
                        }
                        ma.Movie = value;
                    }
                }
                dbContext.Movies.Add(value);
                await dbContext.SaveChangesAsync();
                return Created(new Uri("/api/movies", UriKind.Relative), value);
            }
        }

        /// <summary>
        /// Update an existing movie's title or description
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/movies/1
        ///     {
        ///         "Description": "This movie recently won an oscar"
        ///     }
        /// </remarks>
        /// <param name="id">The ID of the movie to update</param>
        /// <param name="value">A MovieModel containing the data to change</param>
        /// <returns>The newly updated movie</returns>
        /// <response code="200">The movie was updated successfully</response>
        /// <response code="404">If a single actor with movie with the requested ID cannot be found</response>
        /// <response code="400">If a null model was passed in</response>
        // PUT api/movies/1
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Put(int id, [FromBody] MovieModel value)
        {
            if (value == null)
                return BadRequest();

            using (var dbContext = new MovieDbContext())
            {
                try
                {
                    var movie = dbContext.Movies.Where(m => m.MovieId == id).Include(m => m.MovieActors)
                   .ThenInclude(ma => ma.Actor)
                   .Single();

                   if(movie.Title != value.Title && value.Title != null)
                        movie.Title = value.Title;

                    if(movie.Description != value.Description && value.Description != null)
                        movie.Description = value.Description;

                    await dbContext.SaveChangesAsync();
                    return Ok(movie);
                }
                catch (InvalidOperationException ex)
                {
                    _log.LogError(ex.GetType().FullName + " :: " + ex.Message);
                    return NotFound("Unable to find a single movie with the requested ID");
                }
            }
        }

        /// <summary>
        /// Delete a specified movie
        /// </summary>
        /// <param name="id">The ID of the movie to delete</param>
        /// <returns>Returns a success notification once the movie has been deleted</returns>
        /// <response code="200">The movie was deleted successfully</response>
        /// <response code="404">If a single movie with movie with the requested ID cannot be found</response>
        // DELETE api/movies/1
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            using (var dbContext = new MovieDbContext())
            {
                try
                {
                    var movie = dbContext.Movies.Where(m => m.MovieId == id)
                        .Single();

                    dbContext.Movies.Remove(movie);
                    await dbContext.SaveChangesAsync();

                    return Ok("Movie deleted");
                }
                catch (InvalidOperationException ex)
                {
                    _log.LogError(ex.GetType().FullName + " :: " + ex.Message);
                    return NotFound("Unable to find a single movie with the requested ID");
                }
            }
        }
    }
}