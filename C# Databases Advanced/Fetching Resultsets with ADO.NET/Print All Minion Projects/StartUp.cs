using Initial_Setup;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Print_All_Minion_Name
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var minionNames = new List<string>();

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                string sqlQuery = "SELECT Name FROM Minions";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            minionNames.Add((string)reader[0]);
                        }
                    }
                }
            }

            for (int i = 0; i < minionNames.Count / 2; i++)
            {
                Console.WriteLine(minionNames[i]);
                Console.WriteLine(minionNames[minionNames.Count - 1 - i]);
            }
        }
    }
}