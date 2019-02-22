using Initial_Setup;
using System;
using System.Data.SqlClient;

namespace Add_Minion
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            string[] minionInfo = Console.ReadLine().Split();
            string[] villainInfo = Console.ReadLine().Split();

            string minionName = minionInfo[1];
            int minionAge = int.Parse(minionInfo[2]);
            string townName = minionInfo[3];

            string villainName = villainInfo[1];

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                int? townId = GetTownByName(townName, connection);

                if (townId == null)
                {
                    AddTown(connection, townName);
                }

                townId = GetTownByName(townName, connection);

                int? villainId = GetVillianByName(villainName, connection);

                if (villainId == null)
                {
                    AddVillian(villainName, connection);
                }

                villainId = GetVillianByName(villainName, connection);

                int? minionId = GetMinionByName(minionName, connection);

                if (minionId == null)
                {
                    string insertMinion = "INSERT INTO Minions (Name, Age, TownId) VALUES (@name, @age, @townId)";

                    using (SqlCommand command = new SqlCommand(insertMinion, connection))
                    {
                        command.Parameters.AddWithValue("@name", minionName);
                        command.Parameters.AddWithValue("@age", minionAge);
                        command.Parameters.AddWithValue("@name", townId);
                        command.ExecuteNonQuery();
                    }
                }

                minionId = GetMinionByName(minionName, connection);

                AddMinionToVillain(minionId, villainId, connection, minionName, villainName);
            }
        }

        private static int? GetMinionByName(string minionName, SqlConnection connection)
        {
            string selectVillianId = "SELECT Id FROM Minions WHERE Name = @Name";

            using (SqlCommand command = new SqlCommand(selectVillianId, connection))
            {
                command.Parameters.AddWithValue("@Name", minionName);
                return (int?)command.ExecuteScalar();
            }
        }

        private static void AddMinionToVillain(int? minionId, int? villainId, SqlConnection connection, string minionName, string villainName)
        {
            string insertServantMinion = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@villainId, @minionId)";

            using (SqlCommand command = new SqlCommand(insertServantMinion, connection))
            {
                command.Parameters.AddWithValue("@villainId", villainId);
                command.Parameters.AddWithValue("@minionId", minionId);
                command.ExecuteScalar();
            }

            Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
        }

        private static void AddVillian(string villainName, SqlConnection connection)
        {
            string insertTown = "INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)";

            using (SqlCommand command = new SqlCommand(insertTown, connection))
            {
                command.Parameters.AddWithValue("@villainName", villainName);
                command.ExecuteNonQuery();
            }

            Console.WriteLine($"Villain {villainName} was added to the database.");
        }

        private static int? GetVillianByName(string villainName, SqlConnection connection)
        {
            string selectVillianId = "SELECT Id FROM Villains WHERE Name = @Name";

            using (SqlCommand command = new SqlCommand(selectVillianId, connection))
            {
                command.Parameters.AddWithValue("@Name", villainName);
                return (int?)command.ExecuteScalar();
            }
        }

        private static void AddTown(SqlConnection connection, string townName)
        {
            string insertTown = "INSERT INTO Towns (Name) VALUES (@townName)";

            using (SqlCommand command = new SqlCommand(insertTown, connection))
            {
                command.Parameters.AddWithValue("@townName", townName);
                command.ExecuteNonQuery();
            }

            Console.WriteLine($"Town {townName} was added to the database.");
        }

        private static int? GetTownByName(string townName, SqlConnection connection)
        {
            string selectTownId = "SELECT Id FROM Towns WHERE Name = @townName";

            using (SqlCommand command = new SqlCommand(selectTownId, connection))
            {
                command.Parameters.AddWithValue("@townName", townName);
                return (int?)command.ExecuteScalar();
            }
        }
    }
}