using Initial_Setup;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace Increase_Minion_Age
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            int[] id = Console.ReadLine().Split().Select(int.Parse).ToArray();

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                for (int i = 0; i < id.Length; i++)
                {
                    string updateQuery =
                    @" UPDATE Minions
                    SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1
                    WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id[i]);
                        command.ExecuteNonQuery();
                    }
                }

                string selectQuery = "SELECT Name, Age FROM Minions";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader[0] + " " + reader[1]);
                        }
                    }
                }
            }
        }
    }
}