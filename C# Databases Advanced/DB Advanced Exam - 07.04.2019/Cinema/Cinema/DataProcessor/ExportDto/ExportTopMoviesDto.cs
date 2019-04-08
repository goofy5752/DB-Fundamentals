using Cinema.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.DataProcessor.ExportDto
{
    public class ExportTopMoviesDto
    {
        public string MovieName { get; set; }

        public string Rating { get; set; }

        public string TotalIncomes { get; set; }

        public CustomerDto[] Customers { get; set; }
    }
}