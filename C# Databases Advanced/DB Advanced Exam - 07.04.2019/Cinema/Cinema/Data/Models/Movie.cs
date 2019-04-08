using Cinema.Data.Models.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cinema.Data.Models
{
    public class Movie
    {
        public Movie()
        {
            this.Projections = new List<Projection>();
        }

        public int Id { get; set; }

        [Required]
        [MinLength(3), MaxLength(20)]
        public string Title { get; set; }

        [Required]
        public Genre Genre { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        [Range(1, 10)]
        [Required]
        public double Rating { get; set; }

        [MinLength(3), MaxLength(20)]
        public string Director { get; set; }

        public ICollection<Projection> Projections { get; set; }
    }
}
