using MySqlConnector;
using WeatherWebApp.Models;

namespace WeatherWebApp.Repos
{
    public class WeatherRepo
    {

        public async Task Save(Weather weather)
        {

            await using var conn = DatabaseService.DatabaseService.CreateConnection();
            await conn.OpenAsync();

            const string insertWeather = @"
                INSERT INTO weather (
                    city_id,
                    location,
                    created_at,
                    weather_condition,
                    temperature,
                    temp_feels_like,
                    humidity
                )
                VALUES (
                    @city_id,
                    @location,
                    @created_at,
                    @weather_condition,
                    @temperature,
                    @temp_feels_like,
                    @humidity
                );";

            await using var command = new MySqlCommand(insertWeather, conn);

            command.Parameters.AddWithValue("@city_id", weather.CityId);
            command.Parameters.AddWithValue("@location", weather.Location);
            command.Parameters.AddWithValue("@created_at", weather.CreatedAt);
            command.Parameters.AddWithValue("@weather_condition", weather.Condition);
            command.Parameters.AddWithValue("@temperature", weather.Temperature);
            command.Parameters.AddWithValue("@temp_feels_like", weather.TempFeelsLike);
            command.Parameters.AddWithValue("@humidity", weather.Humidity);

            await command.ExecuteNonQueryAsync();
        }


        public async Task<List<Weather>> GetAll()
        {
            var allWeather = new List<Weather>();

            await using var conn = DatabaseService.DatabaseService.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"
                SELECT 
                    * 
                FROM weather;";

            await using var command = new MySqlCommand(sql, conn);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var weather = new Weather(
                    reader.GetInt32("city_id"),
                    reader.GetString("location"),
                    reader.GetString("weather_condition"),                    
                    reader.GetDouble("temperature"),
                    reader.GetDouble("temp_feels_like"),
                    reader.GetInt32("humidity"),
                    reader.GetDateTime("created_at")
                );

                allWeather.Add(weather);
            }

            return allWeather;
        }


    }
}