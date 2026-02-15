namespace WeatherWebApp.Models
{
    public class WeatherViewModel
    {
        public string? City { get; set; }
        public double CurrentTemperature { get; set; }
        public string? CurrentCondition { get; set; }
        public List<ForecastItem> Forecast { get; set; } = new List<ForecastItem>();
    }

    public class ForecastItem
    {
        public DateTime Date { get; set; }
        public double Temperature { get; set; }
        public string? Condition { get; set; }
    }
}
