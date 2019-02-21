using Initial_Setup;
using System;
using System.Data.SqlClient;

namespace Minion_Names
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            int id = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string villainNameQuery = "SELECT Name FROM Villains WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(villainNameQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    string villainName = (string)command.ExecuteScalar();

                    if (villainName == null)
                    {
                        Console.WriteLine($"No villain with ID {id} exists in the database.");
                    }

                    Console.WriteLine($"Villain: {villainName}");
                }

                string minionsQuery = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name";

                using (SqlCommand command = new SqlCommand(minionsQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            long rowNumber = (long)dataReader[0];
                            string name = (string)dataReader[1];
                            int age = (int)dataReader[2];

                            Console.WriteLine($"{rowNumber}. {name} {age}");
                        }

                        if (!dataReader.HasRows)
                        {
                            Console.WriteLine("(no minions)");
                        }
                    }
                }
            }
        }
    }
}
