namespace Cinema.DataProcessor
{
    using AutoMapper;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var movies = JsonConvert.DeserializeObject<ImportMovieDto[]>(jsonString);

            var sb = new StringBuilder();

            var validMovies = new List<Movie>();

            foreach (var movie in movies)
            {
                var genre = Enum.TryParse(movie.Genre, out Genre Genree);

                if (IsValid(movie) && IsValid(genre))
                {
                    sb.AppendLine(string.Format(SuccessfulImportMovie, movie.Title, movie.Genre, movie.Rating.ToString("F2")));

                    validMovies.Add(new Movie
                    {
                        Title = movie.Title,
                        Genre = Enum.Parse<Genre>(movie.Genre),
                        Duration = TimeSpan.Parse(movie.Duration),
                        Rating = movie.Rating,
                        Director = movie.Director
                    });
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            context.AddRange(validMovies);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var halls = JsonConvert.DeserializeObject<ImportHallSeats[]>(jsonString);

            var sb = new StringBuilder();

            var validHallSeats = new List<Hall>();

            foreach (var hall in halls)
            {
                if (IsValid(hall))
                {
                    if (hall.Is4Dx && hall.Is3D)
                    {
                        sb.AppendLine($"Successfully imported {hall.Name}(4Dx/3D) with {hall.Seats} seats!");
                    }
                    else if (hall.Is4Dx)
                    {
                        sb.AppendLine($"Successfully imported {hall.Name}(4Dx) with {hall.Seats} seats!");
                    }
                    else if (hall.Is3D)
                    {
                        sb.AppendLine($"Successfully imported {hall.Name}(3D) with {hall.Seats} seats!");
                    }
                    else
                    {
                        sb.AppendLine($"Successfully imported {hall.Name}(Normal) with {hall.Seats} seats!");
                    }

                    var seatList = new List<Seat>();

                    for (int i = 0; i < hall.Seats; i++)
                    {
                        seatList.Add(new Seat());
                    }

                    validHallSeats.Add(new Hall
                    {
                        Name = hall.Name,
                        Is4Dx = hall.Is4Dx,
                        Is3D = hall.Is3D,
                        Seats = seatList
                    });
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            context.AddRange(validHallSeats);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            XmlSerializer xml = new XmlSerializer(typeof(ImportProjectionDto[]), new XmlRootAttribute("Projections"));

            var projectionsDto = (ImportProjectionDto[])xml.Deserialize(new StringReader(xmlString));

            var projections = new List<Projection>();

            var sb = new StringBuilder();

            foreach (var projection in projectionsDto)
            {
                var salesToAdd = Mapper.Map<Projection>(projection);

                if (!IsValid(salesToAdd))
                {
                    sb.AppendLine(ErrorMessage);
                }
                else
                {
                    var movie = context
                        .Movies
                        .Select(m => new
                        {
                            m.Id,
                            m.Title
                        })
                        .ToList();

                    var moviesCount = context.Movies.Count();
                    var hallsCount = context.Halls.Count();

                    if (salesToAdd.HallId < 1 || salesToAdd.HallId > hallsCount || salesToAdd.MovieId > moviesCount || salesToAdd.MovieId < 1)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    string neededTitle = movie.Where(x => x.Id == salesToAdd.MovieId).First().Title;

                    sb.AppendLine(string.Format(SuccessfulImportProjection, neededTitle, salesToAdd.DateTime.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)));
                    projections.Add(salesToAdd);
                }
            }

            context.Projections.AddRange(projections);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            XmlSerializer xml = new XmlSerializer(typeof(ImportCustomerTicketsDto[]), new XmlRootAttribute("Customers"));

            var customersTicketsDto = (ImportCustomerTicketsDto[])xml.Deserialize(new StringReader(xmlString));

            var customerTickets = new List<Customer>();

            var sb = new StringBuilder();

            foreach (var customer in customersTicketsDto)
            {
                if (IsValid(customer) && customer.Tickets.All(IsValid))
                {
                    sb.AppendLine(string.Format(SuccessfulImportCustomerTicket, customer.FirstName, customer.LastName, customer.Tickets.Count()));

                    var Customer = new Customer()
                    {
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        Age = customer.Age,
                        Balance = customer.Balance,
                        Tickets = customer.Tickets.Select(t => new Ticket
                        {
                            ProjectionId = t.ProjectionId,
                            Price = t.Price
                        })
                        .ToArray()
                    };
                    customerTickets.Add(Customer);

                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            context.Customers.AddRange(customerTickets);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);

            var validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext,
            validationResults, true);
        }
    }
}