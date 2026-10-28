using Microsoft.Data.SqlClient;

namespace TrainingManagementSystem.Data
{
    public class DbConnectionFactory
    {
        private readonly string connectionString =
            @"Server=.\SQLEXPRESS;Database=TrainingManagementDB;Integrated Security=True;TrustServerCertificate=True;";

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}