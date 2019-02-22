using Initial_Setup;
using System;
using System.Data.SqlClient;

namespace Remove_Villain
{
    class Program
    {
        static void Main(string[] args)
        {
            int id = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string villainName;

                string selectVillainName = "SELECT Name FROM Villains WHERE Id = @villainId";

                using (SqlCommand command = new SqlCommand(selectVillainName, connection))
                {
                    command.Parameters.AddWithValue(@"villainId", id);
                    villainName = (string)command.ExecuteScalar();

                    if (villainName == null)
                    {
                        Console.WriteLine("No such villain was found.");
                        return;
                    }
                }

                int deletedMinions = DeleteMinionsById(id, connection);

                DeleteVillainsById(id, connection);

                Console.WriteLine($"{villainName} was deleted.");
                Console.WriteLine($"{deletedMinions} minions were released.");
            }
        }

        private static void DeleteVillainsById(int id, SqlConnection connection)
        {
            string deleteVillains =
                    @"DELETE FROM Villains
                      WHERE Id = @villainId";

            using (SqlCommand command = new SqlCommand(deleteVillains, connection))
            {
                command.Parameters.AddWithValue("@villainId", id);
                command.ExecuteNonQuery();
            }
        }

        private static int DeleteMinionsById(int id, SqlConnection connection)
        {
            string deleteMinions =
                    @"DELETE FROM MinionsVillains 
                      WHERE VillainId = @villainId";

            using (SqlCommand command = new SqlCommand(deleteMinions, connection))
            {
                command.Parameters.AddWithValue("@villainId", id);
                return command.ExecuteNonQuery();
            }
        }
    }
}