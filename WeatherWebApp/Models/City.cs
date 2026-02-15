namespace WeatherWebApp.Models
{
    public class City(int cityId, string cityName, double latitude, double longitude, string country, string province)
    {
        public int cityId {get; set;} = cityId;
        public string CityName {get; set;} = cityName;
        public double Latitude { get; set; } = latitude;
        public double Longitude { get; set; } = longitude;
        public string Country { get; set; } = country;
        public String Province { get; set; } = province;

        public override string ToString()
        {
            return $"{CityName}, Coordinates(lat:lon) = {Latitude} : {Longitude}, {Country}, {Province}";
        }
    }
}
