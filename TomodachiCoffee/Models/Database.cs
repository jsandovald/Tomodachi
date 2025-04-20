using MySql.Data.MySqlClient;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

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

            var paramNames = Enumerable.Range(0, columnCount)
                                       .Select(i => $"@param{i}")
                                       .ToList();

            string sql = $"INSERT INTO `{tableName}` VALUES ({string.Join(",", paramNames)})";

            foreach (DataRow row in data.Rows)
            {
                var parameters = new List<MySqlParameter>();

                for (int i = 0; i < columnCount; i++)
                {
                    object value = row[i] ?? DBNull.Value;

                    if (value is string strValue)
                    {
                        value = CleanString(strValue);
                    }

                    parameters.Add(new MySqlParameter(paramNames[i], value));
                }

                using var cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddRange(parameters.ToArray());
                cmd.ExecuteNonQuery();
            }
        }

        private static string CleanString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // 2. Eliminar emojis (fuera del rango básico de caracteres Unicode)
            string cleaned = Regex.Replace(input, @"[\p{Cs}\p{So}\p{Co}]", "emojis");

            // 3. Eliminar caracteres especiales (excepto letras, números, espacios, comas y puntos)
            cleaned = Regex.Replace(cleaned, @"[^a-zA-Z0-9\u00C0-\u00FF\s\.,:\-_/]", "");

            return cleaned;
        }
    }
}
