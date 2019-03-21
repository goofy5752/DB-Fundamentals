namespace BookShop
{
    using BookShop.Initializer;
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Z.EntityFramework.Plus;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            { 
                Console.WriteLine(RemoveBooks(db));
            }
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var restriction = Enum.Parse<AgeRestriction>(command, true);

            var booksWithRestriction = context.Books
                .Where(b => b.AgeRestriction == restriction)
                .Select(b => b.Title)
                .OrderBy(x => x)
                .ToList();

            return string.Join(Environment.NewLine, booksWithRestriction);
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var goldenBooks = context
                .Books
                .Where(b => b.Copies < 5000 && b.EditionType == EditionType.Gold)
                .Select(b => new
                {
                    b.BookId,
                    b.Title
                })
                .OrderBy(b => b.BookId)
                .ToList();

            return string.Join(Environment.NewLine, goldenBooks.Select(b => b.Title));
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var sb = new StringBuilder();

            var booksByPrice = context
                .Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToList();

            foreach (var book in booksByPrice)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var booksNotRealesedIn = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .Select(b => new
                {
                    b.Title,
                    b.BookId
                })
                .OrderBy(b => b.BookId)
                .ToList();

            return string.Join(Environment.NewLine, booksNotRealesedIn.Select(b => b.Title));
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var splittedInput = input.ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var titleToAdd = context
                .Books
                .Where(b => b.BookCategories.Any(c => splittedInput.Contains(c.Category.Name.ToLower())))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            return string.Join(Environment.NewLine, titleToAdd);
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var parsedDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);


            var neededBooks = context
                .Books
                .Where(b => b.ReleaseDate.Value < parsedDate)
                .OrderByDescending(b => b.ReleaseDate.Value)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price,
                })
                .ToList();

            var result = string.Join(Environment.NewLine, neededBooks.Select(book => $"{book.Title} - {book.EditionType} - ${book.Price:F2}"));

            return result;
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var neededAuthors = context.Authors
                .Where(x => x.FirstName.EndsWith(input))
                .Select(x => $"{x.FirstName} {x.LastName}")
                .OrderBy(x => x)
                .ToList();

            return string.Join(Environment.NewLine, neededAuthors);
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var bookSearch = context
                .Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            return string.Join(Environment.NewLine, bookSearch);
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var booksAuthors = context
                .Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    AuthorName = b.Author.FirstName + " " + b.Author.LastName,
                    b.Title
                })
                .ToList();

            return string.Join(Environment.NewLine, booksAuthors.Select(b => $"{b.Title} ({b.AuthorName})"));
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var countBooks = context
                .Books
                .Where(b => b.Title.Length > lengthCheck).Count();

            return countBooks;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var totalAuthorsCopies = context
                .Authors
                .Select(b => new
                {
                    AuthorName = b.FirstName + " " + b.LastName,
                    TotalCopies = b.Books.Sum(x => x.Copies)
                })
                .OrderByDescending(x => x.TotalCopies);

            return string.Join(Environment.NewLine, totalAuthorsCopies.Select(a => $"{a.AuthorName} - {a.TotalCopies}"));
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var totalProfit = context
                .Categories
                .Select(c => new
                {
                    c.Name,
                    TotalProfit = c.CategoryBooks.Sum(cb => cb.Book.Price * cb.Book.Copies)
                })
                .OrderByDescending(c => c.TotalProfit)
                .ThenBy(c => c.Name)
                .ToList();

            return string.Join(Environment.NewLine, totalProfit.Select(x => $"{x.Name} ${x.TotalProfit:F2}"));
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories.Count();

            var sb = new StringBuilder();

            var recentBooks = context
                .Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    CategoryBooks = c.CategoryBooks.Select(cb => new
                    {
                        BookTitle = cb.Book.Title,
                        BookReleaseDate = cb.Book.ReleaseDate.Value
                    })
                    .OrderByDescending(cb => cb.BookReleaseDate)
                    .Take(3)
                })
                .OrderBy(c => c.CategoryName)
                .ToList();

            foreach (var book in recentBooks)
            {
                sb.AppendLine($"--{book.CategoryName}");
                foreach (var item in book.CategoryBooks)
                {
                    sb.AppendLine($"{item.BookTitle} ({item.BookReleaseDate.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year < 2010)
                .Update(x => new Book() { Price = x.Price + 5 });

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var removedBooks = context
                .Books
                .Where(b => b.Copies < 4200)
                .ToList();

            context.Books.RemoveRange(removedBooks);

            context.SaveChanges();

            return removedBooks.Count;
        }
    }
}