using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Ticket")]
    public class TicketDto
    {
        [XmlElement("ProjectionId")]
        [Required]
        public int ProjectionId { get; set; }

        [XmlElement("Price")]
        [Required]
        public decimal Price { get; set; }
    }
}