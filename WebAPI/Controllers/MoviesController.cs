using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private MovieDbContext dbContext;
        // GET api/movies
        [HttpGet]
        public IActionResult Get()
        {
            using (dbContext = new MovieDbContext())
            {
                var movies = dbContext.Movies.Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor);
                var json = JsonConvert.SerializeObject(movies,
                new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

                return Ok(json);
            }
        }

        // GET api/movies/1
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            using (dbContext = new MovieDbContext())
            {
                try
                {
                    var movie = dbContext.Movies.Where(m => m.MovieId == id).Include(m => m.MovieActors)
                   .ThenInclude(ma => ma.Actor)
                   .Single();

                    var json = JsonConvert.SerializeObject(movie,
                        new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

                    return Ok(json);
                }
                catch(InvalidOperationException ex)
                {
                    return NotFound();
                }
               
            }
        }

        // POST api/movies
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/movies/1
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/movies/1
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
