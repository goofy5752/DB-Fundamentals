using AutoMapper;
using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<CarDealerProfile>();
            });

            var xml = File.ReadAllText(@"C:\Users\Admin\Desktop\Programming\Software University\DB-Fundamentals\C# Databases Advanced\Extensible Markup Language - XML\Car Dealer\CarDealer\Datasets\sales.xml");

            using (var context = new CarDealerContext())
            {
                Console.WriteLine(GetSalesWithAppliedDiscount(context));
            }
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xml = new XmlSerializer(typeof(ImportSupplierDto[]), new XmlRootAttribute("Suppliers"));

            var suppliersDto = (ImportSupplierDto[])xml.Deserialize(new StringReader(inputXml));

            var suppliersList = new List<Supplier>();

            foreach (var supplier in suppliersDto)
            {
                var supplierToAdd = Mapper.Map<Supplier>(supplier);

                suppliersList.Add(supplierToAdd);
            }

            context.Suppliers.AddRange(suppliersList);
            context.SaveChanges();

            return $"Successfully imported {suppliersList.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlSerializer xml = new XmlSerializer(typeof(ImportPartDto[]), new XmlRootAttribute("Parts"));

            var partsDto = (ImportPartDto[])xml.Deserialize(new StringReader(inputXml));

            var partsList = new List<Part>();

            var supplierIds = context.Suppliers.Select(s => s.Id).ToList();

            foreach (var part in partsDto)
            {
                var partToAdd = Mapper.Map<Part>(part);

                if (!supplierIds.Contains(partToAdd.SupplierId))
                {
                    continue;
                }

                partsList.Add(partToAdd);
            }

            context.Parts.AddRange(partsList);
            context.SaveChanges();

            return $"Successfully imported {partsList.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer xml = new XmlSerializer(typeof(ImportCarDto[]), new XmlRootAttribute("Cars"));

            var carsDto = (ImportCarDto[])xml.Deserialize(new StringReader(inputXml));

            var cars = new List<Car>();

            var partIds = context.Parts.Select(p => p.Id).ToList();

            foreach (var car in carsDto)
            {
                foreach (var part in car.Parts)
                {
                    if (partIds.Contains(part))
                    {
                        car.Parts.Add(part);
                    }
                }
                var carsToAdd = Mapper.Map<Car>(car);

                cars.Add(carsToAdd);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xml = new XmlSerializer(typeof(ImportCustomerDto[]), new XmlRootAttribute("Customers"));

            var customersDto = (ImportCustomerDto[])xml.Deserialize(new StringReader(inputXml));

            var customers = new List<Customer>();

            foreach (var customer in customersDto)
            {
                var customersToAdd = Mapper.Map<Customer>(customer);

                customers.Add(customersToAdd);
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer xml = new XmlSerializer(typeof(ImportSaleDto[]), new XmlRootAttribute("Sales"));

            var salesDto = (ImportSaleDto[])xml.Deserialize(new StringReader(inputXml));

            var sales = new List<Sale>();

            var carIds = context.Cars.Select(c => c.Id).ToList();

            var customerIds = context.Customers.Select(c => c.Id).ToList();

            foreach (var sale in salesDto)
            {
                var salesToAdd = Mapper.Map<Sale>(sale);

                if (!carIds.Contains(salesToAdd.CarId))
                {
                    continue;
                }

                if (!customerIds.Contains(salesToAdd.CustomerId))
                {
                    continue;
                }

                sales.Add(salesToAdd);
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var productsInRange = context
                .Cars
                .Where(c => c.TravelledDistance > 2000000)
                .Select(c => new ExportCarWithDistanceDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ToArray();

            var xml = new XmlSerializer(typeof(ExportCarWithDistanceDto[]), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", "")
            });

            xml.Serialize(new StringWriter(sb), productsInRange, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var productsInRange = context
                .Cars
                .Where(c => c.Make == "BMW")
                .Select(c => new ExportCarFromMakeBMWDto
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ToArray();

            var xml = new XmlSerializer(typeof(ExportCarFromMakeBMWDto[]), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", "")
            });

            xml.Serialize(new StringWriter(sb), productsInRange, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var productsInRange = context
                .Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new ExportLocalSupplierDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            var xml = new XmlSerializer(typeof(ExportLocalSupplierDto[]), new XmlRootAttribute("suppliers"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", "")
            });

            xml.Serialize(new StringWriter(sb), productsInRange, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithPartsList = context
                .Cars
                .Select(c => new ExportCarWithHerListOfPartsDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars.Select(pc => new PartsDto
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price
                    })
                    .OrderByDescending(pc => pc.Price)
                    .ToArray()
                })
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ToArray();
                

            var xml = new XmlSerializer(typeof(ExportCarWithHerListOfPartsDto[]), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", "")
            });

            xml.Serialize(new StringWriter(sb), carsWithPartsList, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var saleByCustomer = context
                .Customers
                .Where(c => c.Sales.Any())
                .Select(c => new ExportTotalSaleByCustomerDto()
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(pc => pc.Part.Price))
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToArray();

            var xml = new XmlSerializer(typeof(ExportTotalSaleByCustomerDto[]), new XmlRootAttribute("customers"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", "")
            });

            xml.Serialize(new StringWriter(sb), saleByCustomer, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var salesWithDiscount = context
                .Sales
                .Where(s => s.Car != null && s.Customer != null)
                .Select(s => new ExportSaleWithAppliedDiscountDto()
                {
                    Make = s.Car.Make,
                    Model = s.Car.Model,
                    TravelledDistance = s.Car.TravelledDistance,
                    Discount = s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartCars.Sum(pc => pc.Part.Price),
                    PriceWithDiscount = s.Car.PartCars.Sum(pc => pc.Part.Price) - (s.Car.PartCars.Sum(pc => pc.Part.Price) * (s.Discount / 100))
                })
                .ToArray();

            var xml = new XmlSerializer(typeof(ExportSaleWithAppliedDiscountDto[]), new XmlRootAttribute("sales"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", "")
            });

            xml.Serialize(new StringWriter(sb), salesWithDiscount, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}