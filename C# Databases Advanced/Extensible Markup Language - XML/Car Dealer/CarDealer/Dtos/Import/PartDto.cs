using System.Xml.Serialization;

namespace CarDealer.Dtos.Import
{
    public class PartDto
    {
        [XmlAttribute("partId")]
        public int PartId { get; set; }
    }
}