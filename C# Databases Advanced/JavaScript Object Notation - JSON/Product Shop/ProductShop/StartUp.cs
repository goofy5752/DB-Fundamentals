namespace ProductShop
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;
    using ProductShop.Data;
    using ProductShop.Models;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();

            var jsonFile = File.ReadAllText(@"C:\Users\Admin\Desktop\Programming\Software University\DB-Fundamentals\C# Databases Advanced\JavaScript Object Notation - JSON\Product Shop\ProductShop\Datasets\categories-products.json");

            Console.WriteLine(ImportCategoryProducts(context, jsonFile));
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
    }
}