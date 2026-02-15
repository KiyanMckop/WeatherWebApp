using System.Text.Json;
using WeatherWebApp.Models;
using WeatherWebApp.Repos;

namespace WeatherWebApp.Services
{
    public class WeatherService
    {
        const string API_KEY = "4e79b6ca78c3931ef1b2c6ca822ad58f";
        static HttpClient client = new();

        WeatherRepo weatherRepo = new();
        ForecastRepo forecastRepo = new();
        CityRepo cityRepo = new();

        public async Task<Weather> GetCurrentWeather(string city)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={API_KEY}&units=metric";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                using JsonDocument doc = JsonDocument.Parse(responseBody);
                JsonElement root = doc.RootElement;

                int cityId = root.GetProperty("id").GetInt32();
                string locationName = root.GetProperty("name").GetString()!;
                string weatherDescription = root.GetProperty("weather")[0].GetProperty("description").GetString()!;
                double temperature = root.GetProperty("main").GetProperty("temp").GetDouble();
                double feelsLike = root.GetProperty("main").GetProperty("feels_like").GetDouble();
                int humidity = root.GetProperty("main").GetProperty("humidity").GetInt32();

                Weather weather = new(
                    cityId,
                    locationName,
                    weatherDescription,
                    temperature,
                    feelsLike,
                    humidity,
                    DateTime.Now
                );

                //check if the city id does not exist in the database
                //write the new city to the cities table
                if (!await cityRepo.ExistsById(weather.CityId))
                {
                    City cityDetails = await GetCityDetails(weather.Location);
                    cityDetails.cityId = cityId;
                    await cityRepo.Save(cityDetails);
                }

                await weatherRepo.Save(weather);

                return weather;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                throw;
            }


        }

        public async Task<List<Forecast>> Get5DayForecast(string city)
        {
            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={API_KEY}&units=metric";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                using JsonDocument doc = JsonDocument.Parse(responseBody);
                JsonElement root = doc.RootElement;


                int cityId = root.GetProperty("city").GetProperty("id").GetInt32();
                string locationName = root.GetProperty("city").GetProperty("name").GetString()!;


                List<Forecast> allForecast = new();

                foreach (JsonElement item in root.GetProperty("list").EnumerateArray())
                {
                    DateTime forecastTime =
                        DateTime.Parse(item.GetProperty("dt_txt").GetString()!);

                    double temperature =
                        item.GetProperty("main").GetProperty("temp").GetDouble();

                    double feelsLike =
                        item.GetProperty("main").GetProperty("feels_like").GetDouble();

                    int humidity =
                        item.GetProperty("main").GetProperty("humidity").GetInt32();

                    string condition =
                        item.GetProperty("weather")[0]
                            .GetProperty("description")
                            .GetString()!;

                    Forecast forecast = new Forecast(
                        cityId,
                        locationName,
                        forecastTime,
                        condition,
                        temperature,
                        feelsLike,
                        humidity
                    );

                    //check if the city id does not exist in the database
                    //write the new city to the cities table
                    if (!await cityRepo.ExistsById(forecast.CityId))
                    {
                        City cityDetails = await GetCityDetails(forecast.Location);
                        cityDetails.cityId = cityId;
                        await cityRepo.Save(cityDetails);
                    }

                    //check if forecast does not exsist
                    //write new forecast entry
                    if (!await forecastRepo.ExistsByEntry(forecast))
                    {
                        await forecastRepo.Save(forecast);
                    }

                    allForecast.Add(forecast);
                }


                return allForecast;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                throw;
            }

        }


        public async Task<Coordinates> GetCoordinates(string city)
        {

            string url = $"https://api.openweathermap.org/geo/1.0/direct?q={city}&limit=1&appid={API_KEY}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                using JsonDocument doc = JsonDocument.Parse(responseBody);
                JsonElement root = doc.RootElement[0];

                Coordinates coordinates = new Coordinates(
                    root.GetProperty("lon").GetDouble(),
                    root.GetProperty("lat").GetDouble()
                );

                return coordinates;


            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                throw;
            }
        }

        public async Task<string> GetLocationName(double longitude, double latitude)
        {
            string url = $"https://api.openweathermap.org/geo/1.0/reverse?lat={latitude}&lon={longitude}&limit=1&appid={API_KEY}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                using JsonDocument doc = JsonDocument.Parse(responseBody);
                JsonElement root = doc.RootElement[0];

                return root.GetProperty("name").GetString()!;

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                throw;
            }
        }

        public async Task<City> GetCityDetails(string city)
        {
            string url = $"https://api.openweathermap.org/geo/1.0/direct?q={city}&limit=1&appid={API_KEY}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                using JsonDocument doc = JsonDocument.Parse(responseBody);
                JsonElement root = doc.RootElement[0];

                City cityDetails = new City(
                    0,
                    root.GetProperty("name").GetString()!,
                    root.GetProperty("lon").GetDouble(),
                    root.GetProperty("lat").GetDouble(),
                    root.GetProperty("country").GetString()!,
                    root.GetProperty("state").GetString()!
                );

                return cityDetails;

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                throw;
            }
        }


        public async Task<WeatherViewModel> GetWeatherForecast(string city)
        {
            try
            {
                // Get current weather
                var currentWeather = await GetCurrentWeather(city);
                
                // Get 5-day forecast
                var forecasts = await Get5DayForecast(city);

                // Convert forecasts to ForecastItem objects
                var forecastItems = forecasts
                    .Select(f => new ForecastItem
                    {
                        Date = f.ForecastTime,
                        Temperature = f.Temperature,
                        Condition = f.Condition
                    })
                    .ToList();

                // Create and return ViewModel
                var model = new WeatherViewModel
                {
                    City = currentWeather.Location,
                    CurrentTemperature = currentWeather.Temperature,
                    CurrentCondition = currentWeather.Condition,
                    Forecast = forecastItems
                };

                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting weather forecast: {ex.Message}");
                throw;
            }
        }


    }
}