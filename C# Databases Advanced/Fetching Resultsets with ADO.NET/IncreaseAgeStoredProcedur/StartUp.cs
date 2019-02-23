using Initial_Setup;
using System;
using System.Data.SqlClient;

namespace IncreaseAgeStoredProcedur
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            int id = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                string execProcedure = "EXEC usp_GetOlder @id";

                using (SqlCommand command = new SqlCommand(execProcedure, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        Console.WriteLine(reader["Name"] + " - " + reader["Age"] + " years old");
                    }
                }
            }
        }
    }
}