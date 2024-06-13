using Core.Data;
using Core.Services;
using FlightSearchApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FlightSearchApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightOffersController : ControllerBase
    {

            private readonly ModelContext _context;
            private readonly AmadeusApiService _amadeusApiService;
            private readonly ILogger<FlightOffersController> _logger;

            public FlightOffersController(ModelContext context, AmadeusApiService amadeusApiService, ILogger<FlightOffersController> logger)
            {
                _context = context;
                _amadeusApiService = amadeusApiService;
                _logger = logger;
            }
       

            [HttpGet("Search")]
            public async Task<ActionResult<List<FlightSearch.Response>>> GetFlightOffers([FromQuery] FlightSearch.Request request)
            {

                var existingOffer = await _context.FlightOffers.FirstOrDefaultAsync(f =>
                                    f.OriginLocationCode == request.OriginLocationCode &&
                                    f.DestinationLocationCode == request.DestinationLocationCode &&
                                    f.DepartureDate == DateTime.Parse(request.DepartureDate) &&
                                    f.ReturnDate == DateTime.Parse(request.ReturnDate) &&
                                    f.NumberOfPassengers == request.NumberOfPassengers &&
                                    f.Currency == request.Currency);

                var response = new List<FlightSearch.Response>();
                if (existingOffer != null)
                {
                    var flightOffers = JsonConvert.DeserializeObject<FlightOfferResponse>(existingOffer.JsonData);
                    foreach(var item in flightOffers.Data)
                    {
                        var departureSegment = item.Itineraries.FirstOrDefault()?.Segments.FirstOrDefault();
                        var returnSegment = item.Itineraries.FirstOrDefault()?.Segments.LastOrDefault();

                        response.Add(new FlightSearch.Response
                        {
                            OriginLocation = departureSegment.Departure.IataCode,
                            DestinationLocation = returnSegment.Arrival.IataCode,
                            DepartureDate = departureSegment.Departure.At,
                            ReturnDate = returnSegment.Arrival.At,
                            NumberOfStops = item.Itineraries.FirstOrDefault()?.Segments.Count - 1 ?? 0,
                            NumberOfPassengers = request.NumberOfPassengers,
                            NumberOfBookableSeats = item.NumberOfBookableSeats,
                            Currency = item.Price.Currency,
                            TotalPrice = item.Price.GrandTotal
                        });
                    }

                    return Ok(response.GroupBy(offer => new
                    {
                        offer.OriginLocation,
                        offer.DestinationLocation,
                        offer.DepartureDate,
                        offer.ReturnDate,
                        offer.Currency,
                        offer.TotalPrice
                    }).Select(group => group.First()));
            }
            else
            {
                var flightOfferJson = await _amadeusApiService.GetFlightOffersAsync(request.OriginLocationCode, request.DestinationLocationCode, request.DepartureDate, request.ReturnDate, request.NumberOfPassengers, request.Currency);

                if (flightOfferJson != null)
                {
                    var flightOffer = new Core.Models.FlightOffer
                    {
                        OriginLocationCode = request.OriginLocationCode,
                        DestinationLocationCode = request.DestinationLocationCode,
                        DepartureDate = DateTime.Parse(request.DepartureDate),
                        ReturnDate = DateTime.Parse(request.ReturnDate),
                        NumberOfPassengers = request.NumberOfPassengers,
                        Currency = request.Currency,
                        JsonData = flightOfferJson
                    };

                    _context.FlightOffers.AddRange(flightOffer);
                    await _context.SaveChangesAsync();

                    var flightOffers = JsonConvert.DeserializeObject<FlightOfferResponse>(flightOfferJson);
                    foreach (var item in flightOffers.Data)
                    {
                        var departureSegment = item.Itineraries.FirstOrDefault()?.Segments.FirstOrDefault();
                        var returnSegment = item.Itineraries.FirstOrDefault()?.Segments.LastOrDefault();

                        response.Add(new FlightSearch.Response
                        {
                            OriginLocation = departureSegment.Departure.IataCode,
                            DestinationLocation = returnSegment.Arrival.IataCode,
                            DepartureDate = departureSegment.Departure.At,
                            ReturnDate = returnSegment.Arrival.At,
                            NumberOfStops = item.Itineraries.FirstOrDefault()?.Segments.Count - 1 ?? 0,
                            NumberOfPassengers = request.NumberOfPassengers,
                            NumberOfBookableSeats = item.NumberOfBookableSeats,
                            Currency = item.Price.Currency,
                            TotalPrice = item.Price.GrandTotal
                        });
                    }

                    return Ok(response.GroupBy(offer => new
                    {
                        offer.OriginLocation,
                        offer.DestinationLocation,
                        offer.DepartureDate,
                        offer.ReturnDate,
                        offer.Currency,
                        offer.TotalPrice
                    }).Select(group => group.First()));

                }
            }

            return Ok();

        }
    }
}
