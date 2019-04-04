using AutoMapper;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<ProductShopProfile>();
            });

            string path = File.ReadAllText(@"C:\Users\Admin\Desktop\Programming\Software University\DB-Fundamentals\C# Databases Advanced\Extensible Markup Language - XML\Product Shop\ProductShop\Datasets\categories-products.xml");

            using (var context = new ProductShopContext())
            {
                Console.WriteLine(GetUsersWithProducts(context));
            }
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlSerializer xml = new XmlSerializer(typeof(ImportUserDto[]), new XmlRootAttribute("Users"));

            var usersDto = (ImportUserDto[])xml.Deserialize(new StringReader(inputXml));

            var usersList = new List<User>();

            foreach (var user in usersDto)
            {
                var userToAdd = Mapper.Map<User>(user);

                usersList.Add(userToAdd);
            }

            context.Users.AddRange(usersList);
            context.SaveChanges();

            return $"Successfully imported {usersList.Count}";

        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer xml = new XmlSerializer(typeof(ImportProductDto[]), new XmlRootAttribute("Products"));

            var productsDto = (ImportProductDto[])xml.Deserialize(new StringReader(inputXml));

            var products = new List<Product>();

            foreach (var product in productsDto)
            {
                var productToAdd = Mapper.Map<Product>(product);

                products.Add(productToAdd);
            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XmlSerializer xml = new XmlSerializer(typeof(ImportCategoryDto[]), new XmlRootAttribute("Categories"));

            var categoryDto = (ImportCategoryDto[])xml.Deserialize(new StringReader(inputXml));

            var categories = new List<Category>();

            foreach (var category in categoryDto)
            {
                var categoryToAdd = Mapper.Map<Category>(category);

                if (string.IsNullOrEmpty(categoryToAdd.Name))
                {
                    continue;
                }

                categories.Add(categoryToAdd);
            }

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer xml = new XmlSerializer(typeof(ImportCategoryProductDto[]), new XmlRootAttribute("CategoryProducts"));

            var categoriesDto = (ImportCategoryProductDto[])xml.Deserialize(new StringReader(inputXml));

            var categoriesList = new List<CategoryProduct>();

            foreach (var category in categoriesDto)
            {
                var categoriesToAdd = Mapper.Map<CategoryProduct>(category);

                if (categoriesToAdd.CategoryId <= 0 && categoriesToAdd.CategoryId > 11 
                   || categoriesToAdd.ProductId <= 0 && categoriesToAdd.ProductId > 200)
                {
                    continue;
                }

                categoriesList.Add(categoriesToAdd);
            }

            context.CategoryProducts.AddRange(categoriesList);
            context.SaveChanges();

            return $"Successfully imported {categoriesList.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var productsInRange = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new ExportProductsInRangeDto()
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = p.Buyer.FirstName + " " + p.Buyer.LastName
                })
                .OrderBy(p => p.Price)
                .Take(10)
                .ToArray();

            var xml = new XmlSerializer(typeof(ExportProductsInRangeDto[]), new XmlRootAttribute("Products"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", "")
            });

            xml.Serialize(new StringWriter(sb), productsInRange, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var soldProducts = context
                .Users
                .Where(u => u.ProductsSold.Any())
                .Select(u => new ExportSoldProducts()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Product = u.ProductsSold.Select(ps => new ProductDto
                    {
                        Name = ps.Name,
                        Price = ps.Price
                    })
                    .ToArray()
                })
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ToArray();

            var xml = new XmlSerializer(typeof(ExportSoldProducts[]), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", "")
            });

            xml.Serialize(new StringWriter(sb), soldProducts, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categoriesByProductCount = context
                .Categories
                .Select(c => new ExportCategoriesByProductCountDto
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();

            var xml = new XmlSerializer(typeof(ExportCategoriesByProductCountDto[]), new XmlRootAttribute("Categories"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", "")
            });

            xml.Serialize(new StringWriter(sb), categoriesByProductCount, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersProducts = context
                .Users
                .Where(u => u.ProductsSold.Any())
                .OrderByDescending(u => u.ProductsSold.Count)
                .Select(u => new ExportUsersAndProductsDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProduct = new SoldProductDto
                    {
                        Count = u.ProductsSold.Count,
                        Product = u.ProductsSold.Select(ps => new ProductDto
                        {
                            Name = ps.Name,
                            Price = ps.Price
                        })
                        .OrderByDescending(ps => ps.Price)
                        .ToArray()
                    }
                })
                .Take(10)
                .ToArray();

            var usersDto = new ExportUserDto
            {
                Count = context
                            .Users
                            .Count(u => u.ProductsSold.Any()),
                ExportUsersAndProductsDtos = usersProducts
            };

            var xml = new XmlSerializer(typeof(ExportUserDto), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", "")
            });

            xml.Serialize(new StringWriter(sb), usersDto, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}