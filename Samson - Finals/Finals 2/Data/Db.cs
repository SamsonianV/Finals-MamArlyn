using MySql.Data.MySqlClient;

namespace Finals_2.Data
{
    public class Db
    {
        private readonly string _connectionString =
            "server=localhost;database=borrowsystem;user=root;password=Finals;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
