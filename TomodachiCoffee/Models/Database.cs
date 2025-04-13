using MySql.Data.MySqlClient;
using System.Data;

namespace TomodachiCoffee.Models
{
    public class Database
    {
        private const string ConnectionString = "Server=localhost;Port=3306;Database=bd_cafeteria;Uid=root;Pwd=tomodachi426;";

        public static MySqlConnection GetConnection()
        {
            var connection = new MySqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        public static DataTable ExecuteQuery(string query)
        {
            using var connection = GetConnection();
            using var command = new MySqlCommand(query, connection);
            using var adapter = new MySqlDataAdapter(command);
            var table = new DataTable();
            adapter.Fill(table);
            return table;
        }

        public static void InsertDataTable(DataTable data, string tableName)
        {
            using var connection = GetConnection();

            int columnCount = data.Columns.Count;

            // Crear placeholders de parámetros: @param0, @param1, ..., @paramN
            var paramNames = Enumerable.Range(0, columnCount)
                                       .Select(i => $"@param{i}")
                                       .ToList();

            string sql = $"INSERT INTO `{tableName}` VALUES ({string.Join(",", paramNames)})";

            foreach (DataRow row in data.Rows)
            {
                var parameters = new List<MySqlParameter>();

                for (int i = 0; i < columnCount; i++)
                {
                    parameters.Add(new MySqlParameter(paramNames[i], row[i] ?? DBNull.Value));
                }

                using var cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddRange(parameters.ToArray());
                cmd.ExecuteNonQuery();
            }
        }
    }
}
