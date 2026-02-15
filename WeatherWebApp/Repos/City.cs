using MySqlConnector;
using WeatherWebApp.Models;


namespace WeatherWebApp.Repos
{
    public class CityRepo
    {

        public async Task Save(City city)
        {
            await using var conn = DatabaseService.DatabaseService.CreateConnection();
            await conn.OpenAsync();

            const string insertWeather = @"
                INSERT INTO cities (
                    id,
                    city_name,
                    latitude,
                    longitude,
                    country,
                    province
                )
                VALUES (
                    @id,
                    @city_name,
                    @latitude,
                    @longitude,
                    @country,
                    @province
                );";

            await using var command = new MySqlCommand(insertWeather, conn);

            command.Parameters.AddWithValue("@id", city.cityId);
            command.Parameters.AddWithValue("@city_name", city.CityName);
            command.Parameters.AddWithValue("@latitude", city.Latitude);
            command.Parameters.AddWithValue("@longitude", city.Longitude);
            command.Parameters.AddWithValue("@country", city.Country);
            command.Parameters.AddWithValue("@province", city.Province);

            await command.ExecuteNonQueryAsync();
        }


        public async Task<bool> ExistsById(int id)
        {
            await using var conn = DatabaseService.DatabaseService.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"SELECT EXISTS(SELECT 1 FROM cities WHERE id = @id);";

            await using var command = new MySqlCommand(sql, conn);
            command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;

            var result = await command.ExecuteScalarAsync();

            return Convert.ToBoolean(result);
        }


        public async Task<List<City>> GetAll()
        {
            var cities = new List<City>();

            await using var conn = DatabaseService.DatabaseService.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"
                SELECT 
                    * 
                FROM cities;";

            await using var command = new MySqlCommand(sql, conn);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var city = new City(
                    reader.GetInt32("id"),
                    reader.GetString("city_name"),
                    reader.GetDouble("latitude"),
                    reader.GetDouble("longitude"),
                    reader.GetString("country"),
                    reader.GetString("province")
                );

                cities.Add(city);
            }

            return cities;
        }


    }
}