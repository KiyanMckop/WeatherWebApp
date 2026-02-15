namespace WeatherWebApp.Models
{
    public class Coordinates(double longitude, double latitude)
    {
        public double Longitude{get;set;} = longitude;
        public double Latitude{get;set;} = latitude;

        public override string ToString()
        {
            return $"Latitude = {Latitude} : Longitude{Longitude}";
        }
    }
}
