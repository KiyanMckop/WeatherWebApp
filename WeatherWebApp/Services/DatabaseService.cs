using MySqlConnector;

namespace DatabaseService
{

    class DatabaseService
    {

        private const string SERVER = "192.168.0.150";
        private const string DATABASE = "weather";
        private const string USER = "weather_user";
        private const string PASSWORD = "MckopServerWeather";

        public static MySqlConnection CreateConnection()
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = SERVER,
                Port = 3306,
                Database = DATABASE,
                UserID = USER,
                Password = PASSWORD,
            };

            return new MySqlConnection(builder.ConnectionString);
        }

    }

}