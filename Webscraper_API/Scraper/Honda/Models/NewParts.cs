using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webscraper_API.Scraper.Honda.Models
{
    public class NewParts
    {
        public string ID { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string SubCategory { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ImageUrls { get; set; } = string.Empty;
        public string ReferanceImageUrl { get; set; } = string.Empty;
        public string RelatedPartsUrls { get; set; } =string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string PartDescription { get; set; } = string.Empty;
        public string OtherNames { get; set; } = string.Empty;
        public string ItemDimensions { get; set; } = string.Empty;
        public string ItemWeight { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public string FitmentType { get; set; } = string.Empty;
        public string Manufaktur { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        //public PartFitment[] PartFitments { get; set; }
    }
}
