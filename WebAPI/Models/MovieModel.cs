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
        public string Title { get; set; }
        public string Description { get; set; }
        public ICollection<MovieActor> MovieActors { get; set; }
    }
}
