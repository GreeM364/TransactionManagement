using Microsoft.Data.SqlClient;

namespace TransactionManagement.Persistence
{
    /// <summary>
    /// Factory for creating SQL connections.
    /// </summary>
    public class SqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Creates a new SQL connection.
        /// </summary>
        /// <returns>A new instance of <see cref="SqlConnection"/>.</returns>
        public SqlConnection Create()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
