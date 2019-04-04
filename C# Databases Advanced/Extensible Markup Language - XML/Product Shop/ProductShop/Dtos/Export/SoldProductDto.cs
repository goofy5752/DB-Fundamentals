using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    public class SoldProductDto
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlElement("products")]
        public ProductDto[] Product { get; set; }
    }
}