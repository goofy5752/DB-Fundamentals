namespace P01_StudentSystem.Data.Models
{
    using System;

    public class Student
    {
        public Student(string name)
        {
            this.Name = name;
            this.RegisteredOn = DateTime.Now;
        }

        public int StudentId { get; set; }

        public string Name { get; private set; }

        public string PhoneNumber { get; set; }

        public DateTime RegisteredOn { get; private set; }

        public DateTime? Birthday { get; set; }
    }
}