using MySqlConnector;
using WeatherWebApp.Models;

namespace WeatherWebApp.Repos
{
    public class ForecastRepo
    {

        public async Task Save(Forecast forecast)
        {

            await using var conn = DatabaseService.DatabaseService.CreateConnection();
            await conn.OpenAsync();

            const string insertForecast = @"
                INSERT INTO forecast (
                    city_id,
                    location,
                    forecast_time,
                    weather_condition,
                    temperature,
                    temp_feels_like,
                    humidity
                )
                VALUES (
                    @city_id,
                    @location,
                    @forecast_time,
                    @weather_condition,
                    @temperature,
                    @temp_feels_like,
                    @humidity
                );";

            await using var command = new MySqlCommand(insertForecast, conn);

            command.Parameters.AddWithValue("@city_id", forecast.CityId);
            command.Parameters.AddWithValue("@location", forecast.Location);
            command.Parameters.AddWithValue("@forecast_time", forecast.ForecastTime);
            command.Parameters.AddWithValue("@weather_condition", forecast.Condition);
            command.Parameters.AddWithValue("@temperature", forecast.Temperature);
            command.Parameters.AddWithValue("@temp_feels_like", forecast.TempFeelsLike);
            command.Parameters.AddWithValue("@humidity", forecast.Humidity);

            await command.ExecuteNonQueryAsync();
        }


        public async Task<bool> ExistsByEntry(Forecast forecast)
        {
            await using var conn = DatabaseService.DatabaseService.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"SELECT EXISTS(SELECT 1 FROM forecast WHERE city_id = @city_id and location = @location and forecast_time = @forecast_time);";

            await using var command = new MySqlCommand(sql, conn);
            command.Parameters.Add("@city_id", MySqlDbType.Int32).Value = forecast.CityId;
            command.Parameters.Add("@location", MySqlDbType.Int32).Value = forecast.Location;
            command.Parameters.Add("@forecast_time", MySqlDbType.Int32).Value = forecast.ForecastTime;

            var result = await command.ExecuteScalarAsync();

            return Convert.ToBoolean(result);
        }

        public async Task<List<Forecast>> GetAll()
        {
            var forecasts = new List<Forecast>();

            await using var conn = DatabaseService.DatabaseService.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"
                SELECT 
                    * 
                FROM forecast;";

            await using var command = new MySqlCommand(sql, conn);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var forecast = new Forecast(
                    reader.GetInt32("city_id"),                
                    reader.GetString("location"),
                    reader.GetDateTime("forecast_time"),
                    reader.GetString("weather_condition"),
                    reader.GetDouble("temperature"),
                    reader.GetDouble("temp_feels_like"),
                    reader.GetInt32("humidity")
                );

                forecasts.Add(forecast);
            }

            return forecasts;
        }



        //select location, forecast_time, weather_condition, temperature, temp_feels_like, humidity from forecast;

    }
}