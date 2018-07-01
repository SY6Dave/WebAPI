﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using Newtonsoft.Json.Linq;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        // GET api/movies
        [HttpGet]
        public IActionResult Get()
        {
            using (var dbContext = new MovieDbContext())
            {
                var movies = dbContext.Movies.Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor);

                return Ok(movies.ToList());
            }
        }

        // GET api/movies/1
        [HttpGet("{id}")]
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
                    return NotFound("Unable to find a single movie with the requested ID");
                }
            }
        }

        // POST api/movies
        [HttpPost]
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

        // PUT api/movies/1
        [HttpPut("{id}")]
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
                    return NotFound("Unable to find a single movie with the requested ID");
                }
            }
        }

        // DELETE api/movies/1
        [HttpDelete("{id}")]
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
                    return NotFound("Unable to find a single movie with the requested ID");
                }
            }
        }
    }
}