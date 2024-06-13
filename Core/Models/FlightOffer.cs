using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class FlightOffer
    {
        public int Id { get; set; }
        public string OriginLocationCode { get; set; }
        public string DestinationLocationCode { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int NumberOfPassengers { get; set; }
        public string Currency { get; set; }      
        public string JsonData { get; set; }
    }
}
