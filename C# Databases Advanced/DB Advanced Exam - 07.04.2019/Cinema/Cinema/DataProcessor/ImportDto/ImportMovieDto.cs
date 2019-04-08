using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.DataProcessor.ImportDto
{
    public class ImportMovieDto
    {
        [Required]
        [MinLength(3), MaxLength(20)]
        public string Title { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        public string Duration { get; set; }

        [Range(1, 10)]
        [Required]
        public int Rating { get; set; }

        [MinLength(3), MaxLength(20)]
        public string Director { get; set; }
    }
}
