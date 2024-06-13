using Core.Helpers;
using Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Core.Services
{
    public class AmadeusApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string _accessToken;
        private DateTime _tokenExpiration;
        private readonly ILogger<AmadeusApiService> _logger;


        public AmadeusApiService(HttpClient httpClient, IConfiguration configuration, ILogger<AmadeusApiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _tokenExpiration)
            {
                await RefreshTokenAsync();
            }
            return _accessToken;
        }

        private async Task RefreshTokenAsync()
        {
            var clientId = _configuration["Amadeus:ClientId"];
            var clientSecret = _configuration["Amadeus:ClientSecret"];

            var request = new HttpRequestMessage(HttpMethod.Post, "https://test.api.amadeus.com/v1/security/oauth2/token")
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret)
                })
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = System.Text.Json.JsonSerializer.Deserialize<TokenResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _accessToken = tokenResponse.AccessToken;
            _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
        }

        public async Task<string> GetFlightOffersAsync(string origin, string destination, string departureDate, string returnDate, int adults, string currency)
        {
            var token = await GetAccessTokenAsync();

            var requestUrl = $"https://test.api.amadeus.com/v2/shopping/flight-offers?originLocationCode={origin}&destinationLocationCode={destination}&departureDate={departureDate}&returnDate={returnDate}&adults={adults}&currencyCode={currency}";

            _logger.LogInformation("Sending request to Amadeus API: {RequestUrl}", requestUrl);

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.amadeus+json"));

            var response = await _httpClient.SendAsync(request);

            _logger.LogInformation("Received response from Amadeus API: {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error response from Amadeus API: {response.StatusCode} {response.ReasonPhrase} - {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();

            return content;
        }
    }
        
}

