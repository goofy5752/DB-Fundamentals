namespace P01_StudentSystem.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class Course
    {
        public Course(string name)
        {
            this.Name = name;
            this.Resources = new List<Resource>();
        }

        public int CourseId { get; set; }

        public string Name { get; private set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Price { get; set; }

        public ICollection<Resource> Resources { get; set; }
    }
}