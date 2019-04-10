namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var exportPrisonersByCells = context
                .Prisoners
                .Where(p => ids.Contains(p.Id))
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers.Select(po => new
                    {
                        OfficerName = po.Officer.FullName,
                        Department = po.Officer.Department.Name
                    })
                    .OrderBy(po => po.OfficerName)
                    .ToList(),
                    TotalOfficerSalary = p.PrisonerOfficers.Sum(po => po.Officer.Salary)
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToList();

            return JsonConvert.SerializeObject(exportPrisonersByCells, Newtonsoft.Json.Formatting.Indented);
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var prisonersInbox = context
                .Prisoners
                .Where(p => prisonersNames
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .ToArray()
                .Contains(p.FullName))
                .Select(p => new ExportInboxMessageForPrisonerDto()
                {
                    Id = p.Id,
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd"),
                    EncryptedMessages = p.Mails.Select(m => new MessageDto()
                    {
                        Description = ReverseString(m.Description)
                    })
                    .ToArray()
                })
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToArray();

            var xml = new XmlSerializer(typeof(ExportInboxMessageForPrisonerDto[]), new XmlRootAttribute("Prisoners"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", "")
            });

            xml.Serialize(new StringWriter(sb), prisonersInbox, namespaces);

            return sb.ToString().TrimEnd();
        }

        private static string ReverseString(string input)
        {
            var sb = "";

            foreach (var item in input.Reverse())
            {
                sb += item;
            }

            return sb;
        }
    }
}