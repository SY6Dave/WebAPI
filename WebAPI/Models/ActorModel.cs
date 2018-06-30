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
    }
}
