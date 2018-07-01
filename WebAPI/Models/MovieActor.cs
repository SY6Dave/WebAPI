using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class MovieActor
    {
        public int MovieId { get; set; }
        public MovieModel Movie { get; set; }
        public int ActorId { get; set; }
        public ActorModel Actor { get; set; }

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            MovieActor compareModel = (MovieActor)obj;
            return this.MovieId == compareModel.MovieId &&
                this.ActorId == compareModel.ActorId &&
                this.Movie.Equals(compareModel.Movie) &&
                this.Actor.Equals(compareModel.Actor);
        }
    }
}
