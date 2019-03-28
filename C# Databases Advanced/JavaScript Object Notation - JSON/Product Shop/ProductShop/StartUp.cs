namespace ProductShop
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using ProductShop.Data;
    using ProductShop.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();

            var jsonFile = File.ReadAllText(@"C:\Users\Admin\Desktop\Programming\Software University\DB-Fundamentals\C# Databases Advanced\JavaScript Object Notation - JSON\Product Shop\ProductShop\Datasets\products.json");

            Console.WriteLine(GetUsersWithProducts(context));
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var usersToImport = JsonConvert.DeserializeObject<User[]>(inputJson);

            var usersToAdd = new List<User>();

            foreach (var item in usersToImport)
            {
                if (string.IsNullOrWhiteSpace(item.LastName) ||
                    string.IsNullOrEmpty(item.LastName))
                {
                    continue;
                }

                usersToAdd.Add(item);
            }

            context.Users.AddRange(usersToAdd);
            context.SaveChanges();

            return $"Successfully imported {usersToAdd.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var deserializeNeededObjects = JsonConvert.DeserializeObject<Product[]>(inputJson)
                .Where(p => !string.IsNullOrEmpty(p.Name)
                          || !string.IsNullOrWhiteSpace(p.Name))
                .ToArray();

            context.Products.AddRange(deserializeNeededObjects);
            context.SaveChanges();

            return $"Successfully imported {deserializeNeededObjects.Length}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categoriesToAdd =
                JsonConvert.DeserializeObject<Category[]>(inputJson)
                .Where(c => !string.IsNullOrEmpty(c.Name))
                .ToArray();

            context.Categories.AddRange(categoriesToAdd);
            context.SaveChanges();

            return $"Successfully imported {categoriesToAdd.Length}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var validCategoryId = new HashSet<int>(
                context
                .Categories
                .Select(c => c.Id));

            var validProductId = new HashSet<int>(
                context
                .Products
                .Select(p => p.Id));

            var categoriesProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);

            var validCategoriesProducts = new List<CategoryProduct>();

            foreach (var item in categoriesProducts)
            {
                bool isValid = validCategoryId.Contains(item.CategoryId)
                            && validProductId.Contains(item.ProductId);

                if (isValid)
                {
                    validCategoriesProducts.Add(item);
                }
            }

            context.CategoryProducts.AddRange(validCategoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {validCategoriesProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var productsInRange = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    seller = p.Seller.FirstName + " " + p.Seller.LastName
                })
                .ToList();

            return JsonConvert.SerializeObject(productsInRange, Formatting.Indented);
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var soldProducts = context
                .Users
                .Where(u => u.ProductsSold.Any(ps => ps.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold
                    .Where(ps => ps.Buyer != null)
                    .Select(ps => new
                    {
                        name = ps.Name,
                        price = ps.Price,
                        buyerFirstName = ps.Buyer.FirstName,
                        buyerLastName = ps.Buyer.LastName
                    })
                    .ToList()
                })
                .ToList();

            return JsonConvert.SerializeObject(soldProducts, Formatting.Indented);
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categoriesByProductCount = context
                .Categories
                .OrderByDescending(c => c.CategoryProducts.Count)
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoryProducts.Count,
                    averagePrice = $"{c.CategoryProducts.Average(x => x.Product.Price)}",
                    totalRevenue = $"{c.CategoryProducts.Sum(x => x.Product.Price)}"
                })
                .ToList();

            string json = JsonConvert.SerializeObject(categoriesByProductCount, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },

                Formatting = Formatting.Indented
            });

            return json;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersProducts = context
                .Users
                .Where(u => u.ProductsSold.Any(ps => ps.Buyer != null))
                .OrderByDescending(u => u.ProductsSold.Count(ps => ps.Buyer != null))
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = new
                    {
                        count = u.ProductsSold
                        .Count(ps => ps.Buyer != null),
                        products = u.ProductsSold
                        .Where(ps => ps.Buyer != null)
                        .Select(ps => new
                        {
                            name = ps.Name,
                            price = ps.Price
                        })
                        .ToList()
                    }
                })
                .ToList();

            var result = new
            {
                usersCount = usersProducts.Count,
                users = usersProducts
            };

            return JsonConvert.SerializeObject(result, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });
        }
    }
}