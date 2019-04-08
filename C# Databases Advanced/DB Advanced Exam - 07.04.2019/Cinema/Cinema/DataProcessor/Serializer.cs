namespace Cinema.DataProcessor
{
    using System;
    using System.Linq;
    using Cinema.Data.Models;
    using Cinema.DataProcessor.ExportDto;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var movies = context.Movies
                .Where(x => x.Rating >= rating && x.Projections.Select(z => z.Tickets).Any())
                .Take(10)
                .OrderByDescending(x => x.Rating)
                .ThenByDescending(x => x.Projections.Sum(z => z.Tickets.Sum(w => w.Price)))
                .Select(x => new
                {
                    MovieName = x.Title,
                    Rating = $"{x.Rating:F2}",
                    TotalIncomes = $"{x.Projections.Sum(z => z.Tickets.Sum(w => w.Price)):F2}",
                    Customers = x.Projections
                        .SelectMany(z => z.Tickets).Select(w => new
                        {
                            FirstName = w.Customer.FirstName,
                            LastName = w.Customer.LastName,
                            Balance = $"{w.Customer.Balance:F2}"
                        })
                        .OrderByDescending(w => w.Balance)
                        .ThenBy(w => w.FirstName)
                        .ThenBy(w => w.LastName)
                        .ToArray()
                })
                .ToList();

            return JsonConvert.SerializeObject(movies, Formatting.Indented);
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            throw new NotImplementedException();
        }
    }
}