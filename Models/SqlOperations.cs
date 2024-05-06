using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
namespace project_Database.Models
{

    public class SqlOperations
    {
        private readonly string _connectionString;

        public SqlOperations(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Data Source=HASAAN\\MSSQLSERVER01;Initial Catalog=Project;Integrated Security=True;");
        }

        public async Task<DataTable> ValidateCredentials(string email, string password, string role, string Name)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = $"SELECT * FROM {role} WHERE Name = @Name AND Email = @Email AND Password = HASHBYTES('SHA2_256', @Password)";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    
                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<int> Register(string email, string password, string role, string Name)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = $"INSERT INTO {role} (Name ,Email, Password) VALUES (@Name ,@Email, HASHBYTES('SHA2_256', @Password))";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected;
                }
            }
        }

        public async Task<bool> UserExists(string email, string role, string Name)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = $"SELECT * FROM {role} WHERE Email = @Email AND Name= @Name";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {

                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Name", Name);


                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    return reader.HasRows;
                }
            }
        }
    }

}
