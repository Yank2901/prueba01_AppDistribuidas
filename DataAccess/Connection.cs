using System.Data.SqlClient;

namespace DataAccess
{
    public abstract class Connection
    {
        private readonly string _connectionString;

        public Connection()
        {
            _connectionString = @"Data Source=LAPTOP-54CSS05Q\SQLEXPRESS;Initial Catalog=Usuarios;Integrated Security=True";
        }

        protected SqlConnection geConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
