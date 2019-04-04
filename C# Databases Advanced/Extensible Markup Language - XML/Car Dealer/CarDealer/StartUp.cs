using AutoMapper;
using CarDealer.Data;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var xml = File.ReadAllText(@"C:\Users\Admin\Desktop\Programming\Software University\DB-Fundamentals\C# Databases Advanced\Extensible Markup Language - XML\Car Dealer\CarDealer\Datasets\cars.xml");

            using (var context = new CarDealerContext())
            {
                Console.WriteLine(ImportCars(context, xml));
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
    }
}