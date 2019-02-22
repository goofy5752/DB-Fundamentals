using Initial_Setup;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ChangeTownNamesCasing
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            string input = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string updateGivenCities =
                    @"UPDATE Towns
                    SET Name = UPPER(Name)
                    WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";

                using (SqlCommand command = new SqlCommand(updateGivenCities, connection))
                {
                    command.Parameters.AddWithValue("@countryName", input);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("No town names were affected.");
                        return;
                    }
                    Console.WriteLine($"{rowsAffected} town names were affected. ");
                }

                string selectAffectedCountries = 
                    @"SELECT t.Name 
                    FROM Towns as t
                    JOIN Countries AS c ON c.Id = t.CountryCode
                    WHERE c.Name = @countryName";

                var citiesList = new List<string>();
                using (SqlCommand command = new SqlCommand(selectAffectedCountries, connection))
                {
                    command.Parameters.AddWithValue("@countryName", input);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            citiesList.Add((string)reader[0]);
                        }
                    }
                }
                Console.Write("[" + string.Join(", ", citiesList));
                Console.WriteLine("]");
            }
        }
    }
}