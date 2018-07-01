using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class MovieModel
    {
        [Key]
        public int MovieId { get; set; }
        [MinLength(1)]
        public string Title { get; set; }
        [MaxLength(256)]
        public string Description { get; set; }
        public ICollection<MovieActor> MovieActors { get; set; }

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            MovieModel compareModel = (MovieModel)obj;
            return this.MovieId == compareModel.MovieId &&
                this.Title == compareModel.Title &&
                this.Description == compareModel.Description &&
                this.MovieActors.Count() == compareModel.MovieActors.Count();
        }
    }
}
