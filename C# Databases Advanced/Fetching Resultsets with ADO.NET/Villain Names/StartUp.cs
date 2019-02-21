using System;
using System.Data.SqlClient;

namespace Villain_Names
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            string connectionString = Configuration.ConnectionString;

            // Provide the query string with a parameter placeholder.
            string queryString =
                " SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount " +
                "FROM Villains AS v JOIN MinionsVillains AS mv ON v.Id = mv.VillainId " +
                "GROUP BY v.Id, v.Name HAVING COUNT(mv.VillainId) > 3 " +
                "ORDER BY COUNT(mv.VillainId)";

            // Specify the parameter value.

            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);

                // Open the connection in a try/catch block. 
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine("{0} - {1}",
                            reader[0], reader[1]);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.ReadLine();
            }
        }
    }
}