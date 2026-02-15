namespace WeatherWebApp.Models
{
    public class Forecast(int cityId, string location, DateTime forecastTime, string condition, double temperature, double tempFeelsLike, int humidity)
    {

        public int CityId {get; set;} = cityId;
        public string Location { get; set; } = location;
        public DateTime ForecastTime { get; set; } = forecastTime;
        public string Condition { get; set; } = condition;
        public double Temperature { get; set; } = temperature;
        public double TempFeelsLike { get; set; } = tempFeelsLike;
        public int Humidity { get; set; } = humidity;

        public override string ToString()
        {
            return $"{Location} @ {ForecastTime:yyyy-MM-dd HH:mm}: " +
                   $"{Condition}, {Temperature}°C " +
                   $"(Feels like {TempFeelsLike}°C), " +
                   $"Humidity {Humidity}%";
        }
    }
}
