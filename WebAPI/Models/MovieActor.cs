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
    }
}
