namespace SoftJail.DataProcessor
{
    using AutoMapper;
    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
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
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var departments = JsonConvert.DeserializeObject<Department[]>(jsonString);

            var validDepartments = new List<Department>();
            var sb = new StringBuilder();

            foreach (var department in departments)
            {
                if (department.Cells.All(IsValid) && IsValid(department))
                {
                    sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");

                    validDepartments.Add(department);
                }
                else
                {
                    sb.AppendLine("Invalid Data");
                }
            }

            context.Departments.AddRange(validDepartments);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var prisoners = JsonConvert.DeserializeObject<ImportPrisonerDto[]>(jsonString);

            var validPrisoners = new List<Prisoner>();
            var sb = new StringBuilder();

            foreach (var prisoner in prisoners)
            {
                if (prisoner.Mails.All(IsValid) && IsValid(prisoner))
                {
                    var prisonerToAdd = new Prisoner()
                    {
                        FullName = prisoner.FullName,
                        Nickname = prisoner.NickName,
                        Age = prisoner.Age,
                        IncarcerationDate = DateTime.ParseExact(prisoner.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        CellId = prisoner.CellId,
                        Mails = prisoner.Mails.Select(p => new Mail()
                        {
                            Description = p.Description,
                            Sender = p.Sender,
                            Address = p.Address
                        })
                        .ToArray()
                    };

                    if (prisoner.ReleaseDate != null)
                    {
                        prisonerToAdd.ReleaseDate = DateTime.ParseExact(prisoner.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }

                    sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");

                    validPrisoners.Add(prisonerToAdd);
                }
                else
                {
                    sb.AppendLine("Invalid Data");
                }
            }

            context.Prisoners.AddRange(validPrisoners);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            XmlSerializer xml = new XmlSerializer(typeof(ImportOfficersPrisoners[]), new XmlRootAttribute("Officers"));

            var officersPrisonersDto = (ImportOfficersPrisoners[])xml.Deserialize(new StringReader(xmlString));

            var officerPrisoners = new List<Officer>();
            var sb = new StringBuilder();

            foreach (var officerPrisoner in officersPrisonersDto)
            {
                var position = Enum.TryParse(officerPrisoner.Position, out Position Position);
                var weapon = Enum.TryParse(officerPrisoner.Weapon, out Weapon Weapon);

                if (position && weapon && IsValid(officerPrisoner))
                {
                    sb.AppendLine($"Imported {officerPrisoner.Name} ({officerPrisoner.Prisoners.Count()} prisoners)");

                    var officerToAdd = new Officer()
                    {
                        FullName = officerPrisoner.Name,
                        Salary = officerPrisoner.Money,
                        Position = Enum.Parse<Position>(officerPrisoner.Position),
                        Weapon = Enum.Parse<Weapon>(officerPrisoner.Weapon),
                        DepartmentId = officerPrisoner.DepartmentId,
                        OfficerPrisoners = officerPrisoner
                        .Prisoners
                        .Select(p => new OfficerPrisoner()
                        {
                            PrisonerId = p.Id
                        })
                        .ToArray()
                    };

                    officerPrisoners.Add(officerToAdd);
                }
                else
                {
                    sb.AppendLine("Invalid Data");
                }
            }

            context.Officers.AddRange(officerPrisoners);
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