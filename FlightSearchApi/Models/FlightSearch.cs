using System.ComponentModel.DataAnnotations;

namespace FlightSearchApi.Models
{
    public class FlightSearch
    {
        public class Request
        {
            public string OriginLocationCode { get; set; }           
            public string DestinationLocationCode { get; set; }  
            public string DepartureDate { get; set; }           
            public string ReturnDate { get; set; }
            public int NumberOfPassengers { get; set; }
            public string Currency { get; set; }
           
        }
        public static ValidationResult ValidateDepartureDate(string departureDate, ValidationContext context)
        {
            if (DateTime.TryParse(departureDate, out var depDate))
            {
                if (depDate < DateTime.Today)
                {
                    return new ValidationResult("Departure Date must be today or in the future.");
                }
                return ValidationResult.Success;
            }
            return new ValidationResult("Invalid Departure Date.");
        }

        public static ValidationResult ValidateReturnDate(string returnDate, ValidationContext context)
        {
            var instance = context.ObjectInstance as FlightSearch.Request;
            if (DateTime.TryParse(returnDate, out var retDate) && DateTime.TryParse(instance.DepartureDate, out var depDate))
            {
                if (retDate <= depDate)
                {
                    return new ValidationResult("Return Date must be later than Departure Date.");
                }
                return ValidationResult.Success;
            }
            return new ValidationResult("Invalid Return Date.");
        }
    
    public class Response
    {
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        public string DepartureDate { get; set; }
        public string ReturnDate { get; set; }
        public int NumberOfPassengers { get; set; }
        public int NumberOfBookableSeats { get; set; }
        public int NumberOfStops { get; set; }
        public string Currency { get; set; }
        public string TotalPrice { get; set; }
    }
       
 }

    public class FlightOfferResponse
    {
        public List<FlightOffer> Data { get; set; }
        public Meta Meta { get; set; }
        public Dictionaries Dictionaries { get; set; }
    }

    public class FlightOffer
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string Source { get; set; }
        public bool InstantTicketingRequired { get; set; }
        public bool NonHomogeneous { get; set; }
        public bool OneWay { get; set; }
        public bool IsUpsellOffer { get; set; }
        public string LastTicketingDate { get; set; }
        public string LastTicketingDateTime { get; set; }
        public int NumberOfBookableSeats { get; set; }
        public List<Itinerary> Itineraries { get; set; }
        public Price Price { get; set; }
        public PricingOptions PricingOptions { get; set; }
        public List<string> ValidatingAirlineCodes { get; set; }
        public List<TravelerPricing> TravelerPricings { get; set; }
    }

    public class Itinerary
    {
        public string Duration { get; set; }
        public List<Segment> Segments { get; set; }
    }

    public class Segment
    {
        public Departure Departure { get; set; }
        public Arrival Arrival { get; set; }
        public string CarrierCode { get; set; }
        public string Number { get; set; }
        public Aircraft Aircraft { get; set; }
        public Operating Operating { get; set; }
        public string Duration { get; set; }
        public string Id { get; set; }
        public int NumberOfStops { get; set; }
        public bool BlacklistedInEU { get; set; }
    }

    public class Departure
    {
        public string IataCode { get; set; }
        public string Terminal { get; set; }
        public string At { get; set; }
    }

    public class Arrival
    {
        public string IataCode { get; set; }
        public string Terminal { get; set; }
        public string At { get; set; }
    }

    public class Aircraft
    {
        public string Code { get; set; }
    }

    public class Operating
    {
        public string CarrierCode { get; set; }
    }

    public class Price
    {
        public string Currency { get; set; }
        public string Total { get; set; }
        public string Base { get; set; }
        public List<Fee> Fees { get; set; }
        public string GrandTotal { get; set; }
    }

    public class Fee
    {
        public string Amount { get; set; }
        public string Type { get; set; }
    }

    public class PricingOptions
    {
        public List<string> FareType { get; set; }
        public bool IncludedCheckedBagsOnly { get; set; }
    }

    public class TravelerPricing
    {
        public string TravelerId { get; set; }
        public string FareOption { get; set; }
        public string TravelerType { get; set; }
        public Price Price { get; set; }
        public List<FareDetailsBySegment> FareDetailsBySegment { get; set; }
    }

    public class FareDetailsBySegment
    {
        public string SegmentId { get; set; }
        public string Cabin { get; set; }
        public string FareBasis { get; set; }
        public string Class { get; set; }
        public IncludedCheckedBags IncludedCheckedBags { get; set; }
    }

    public class IncludedCheckedBags
    {
        public int Weight { get; set; }
        public string WeightUnit { get; set; }
    }

    public class Meta
    {
        public int Count { get; set; }
        public Links Links { get; set; }
    }

    public class Links
    {
        public string Self { get; set; }
    }

    public class Dictionaries
    {
        public Dictionary<string, Location> Locations { get; set; }
        public Dictionary<string, string> Aircraft { get; set; }
        public Dictionary<string, string> Currencies { get; set; }
        public Dictionary<string, string> Carriers { get; set; }
    }

    public class Location
    {
        public string CityCode { get; set; }
        public string CountryCode { get; set; }
    }
}

