using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class ActorModel
    {
        [Key]
        public int ActorId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public ICollection<MovieActor> MovieActors { get; set; }

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            ActorModel compareModel = (ActorModel)obj;
            return this.ActorId == compareModel.ActorId &&
                this.FirstName == compareModel.FirstName &&
                this.Surname == compareModel.Surname &&
                this.MovieActors.Count() == compareModel.MovieActors.Count();
        }
    }
}
