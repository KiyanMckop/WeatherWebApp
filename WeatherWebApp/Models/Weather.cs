namespace WeatherWebApp.Models
{
    public class Weather(int cityId, string location, string condition, double temperature, double tempFeelsLike, int humidity, DateTime createdAt)
    {
        public int CityId {get; set;} = cityId;
        public string Location { get; set; } = location;
        public string Condition { get; set; } = condition;
        public double Temperature { get; set; } = temperature;
        public double TempFeelsLike { get; set; } = tempFeelsLike;
        public int Humidity { get; set; } = humidity;
        public DateTime CreatedAt { get; set; } = createdAt;

        public override string ToString()
        {
            return $"{Location}: {Condition}, {Temperature}°C (Feels like {TempFeelsLike}°C), Humidity {Humidity}%";
        }
    }
}
