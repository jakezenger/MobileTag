using System;

using System.Data.SqlClient;
using System.Data;
using MobileTag.Models;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace MobileTag
{
    public static class Database
    {
        private static string CONNECTION_STRING = "";
        private static bool initialized = false;
        private delegate void Del(SqlConnection connection);

        private static void Init_() // We can actually add a static constructor to automatically take care of initialization
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "mobiletag-jp.database.windows.net";
            builder.UserID = "eallgood";
            builder.Password = "orangeChicken17";
            builder.InitialCatalog = "MobileTagDB";
            builder.AsynchronousProcessing = true;
            CONNECTION_STRING = builder.ConnectionString.ToString();
            initialized = true;
        }

        private async static Task ExecuteQueryAsync(Func<SqlConnection, Task> del)
        {
            if (!initialized) Init_();
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                try
                {
                    await connection.OpenAsync();
                    await del(connection);
                    connection.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        // can we use player instead of user?  
        public async static Task<int> AddUser(string username, string password, int teamID)
        {
            int available = 0;

            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("AddPlayer", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;
                cmd.Parameters.Add("@teamID", SqlDbType.Int).Value = teamID;
                reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    available = (int)reader["Available"];
                }
                reader.Close();
            };

            await ExecuteQueryAsync(readerProcedure);

            return available;
        }

        public async static Task<int> ValidateLoginCredentials(string username, string password)
        {
            int userValidity = 0;

            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("LookUpUserName", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;
                reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    userValidity = (int)reader["Valid"];
                }
                reader.Close();
            };

            await ExecuteQueryAsync(readerProcedure);

            return userValidity;
        }

        public async static Task<int> GetCellTeam(int cellID)
        {
            int teamID = 0;

            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("GetCellTeam", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@cellID", SqlDbType.Int).Value = cellID;
                reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    teamID = (int)reader["TeamID"];
                }
                reader.Close();
            };

            await ExecuteQueryAsync(readerProcedure);

            return teamID;
        }

        public async static Task<Mine> GetMine(int cellID, int playerID)
        {
            Mine mine = new Mine(cellID, playerID);

            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("GetMine", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@cellID", SqlDbType.Int).Value = cellID;
                cmd.Parameters.Add("@playerID", SqlDbType.Int).Value = playerID;
                reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    mine.Bucket = (int)reader["Bucket"];
                }
                reader.Close();
            };

            await ExecuteQueryAsync(readerProcedure);

            return mine;
        }

        public async static Task EmptyMineBucket(int cellID, int playerID)
        {
            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("EmptyMineBucket", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@cellID", SqlDbType.Int).Value = cellID;
                cmd.Parameters.Add("@playerID", SqlDbType.Int).Value = playerID;
                reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {

                }
                reader.Close();
            };

            await ExecuteQueryAsync(readerProcedure);
        }

        public async static Task AddMine(int playerID, int cellID)
        {
            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("AddMine", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@playerID", SqlDbType.Int).Value = playerID;
                cmd.Parameters.Add("@cellID", SqlDbType.Int).Value = cellID;
                reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {

                }
                reader.Close();
            };

            await ExecuteQueryAsync(readerProcedure);
        }

        public async static Task<int> OperateMine(int playerID, int cellID)
        {
            int bucket = 0;

            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("OperateMine", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@playerID", SqlDbType.Int).Value = playerID;
                cmd.Parameters.Add("@cellID", SqlDbType.Int).Value = cellID;
                reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    bucket = (int)reader["Bucket"];
                }
                reader.Close();
            };

            await ExecuteQueryAsync(readerProcedure);

            return bucket;
        }

        public async static Task AddAntiMine(int playerID, int cellID)
        {
            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("AddAntiMine", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@playerID", SqlDbType.Int).Value = playerID;
                cmd.Parameters.Add("@cellID", SqlDbType.Int).Value = cellID;
                reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {

                }
                reader.Close();
            };

            await ExecuteQueryAsync(readerProcedure);          
        }

        public async static Task<ConcurrentDictionary<int, Mine>> GetMines(int playerID)
        {
            ConcurrentDictionary<int, Mine> mines = new ConcurrentDictionary<int, Mine>();

            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("GetMines", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@playerID", SqlDbType.Int).Value = playerID;
                reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    int cellID = (int)reader["CellID"];
                    int bucket = (int)reader["Bucket"];
                    Mine mine = new Mine(cellID, playerID, bucket);

                    mines.TryAdd(mine.CellID, mine);
                }
                reader.Close();
            };

            await ExecuteQueryAsync(readerProcedure);

            return mines;           
        }

        public async static Task<List<AntiMine>> GetAntiMines(int playerID)
        {
            List<AntiMine> antiMines = new List<AntiMine>();

            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("GetAntiMines", connection); //TODO: Setup Database Stored Procedure
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@playerID", SqlDbType.Int).Value = playerID;
                reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    int cellID = (int)reader["CellID"];
                    AntiMine aMine = new AntiMine(cellID, playerID);
                    antiMines.Add(aMine);
                }
                reader.Close();
            };

            await ExecuteQueryAsync(readerProcedure);

            return antiMines;
        }

        public async static Task<Player> GetPlayer(string username)
        {
            int playerID = 0;
            int teamID = 0;
            string teamName = "";
            ConcurrentDictionary<int, Mine> mines = new ConcurrentDictionary<int, Mine>();
            List<AntiMine> aMines = new List<AntiMine>();
            int cellID = 0;
            Wallet playerWallet = new Wallet();

            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("GetPlayer", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    playerID = (int)reader["PlayerID"];
                    teamID = (int)reader["TeamID"];
                    teamName = (string)reader["TeamName"];
                    cellID = 0; //(int)reader["CellID"];
                    playerWallet.Confinium = (int)reader["Currency"];
                    mines = await GetMines(playerID);
                    aMines = await GetAntiMines(playerID);
                }
                reader.Close();
            };

            await ExecuteQueryAsync(readerProcedure);

            Team team = new Team(teamID, teamName);
            Player player = new Player(playerID, username, team, cellID, mines, aMines, playerWallet);
            return player;
        }

        public async static Task<bool> UpdatePlayerWallet(int playerID, int confinium)
        {

            //TODO: implement database call to update players currency
            //Stored Procedure not yet implemented. Code below may need altering when database is updated.
            try
            {
                Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
                {
                    //SqlConnection connection = new SqlConnection(CONNECTION_STRING);
                    SqlDataReader reader;
                    SqlCommand cmd = new SqlCommand("UpdateWallet", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@amount", SqlDbType.Int).Value = confinium;
                    cmd.Parameters.Add("@playerID", SqlDbType.Int).Value = playerID;


                    reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {

                    }
                    reader.Close();
                    
                };

                await ExecuteQueryAsync(readerProcedure);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to update currency: " + ex.ToString());
            }
            return false;
           
        }

        public async static Task<Cell> GetCell(int cellID)
        {
            decimal lat = 0.00m;
            decimal lng = 0.00m;
            int teamID = 0, holdStrength = 0;

            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("GetCell", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@cellID", SqlDbType.Int).Value = cellID;
                reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    lat = Convert.ToDecimal(reader["Latitude"]);
                    lng = Convert.ToDecimal(reader["Longitude"]);
                    teamID = Convert.ToInt32(reader["TeamID"]);
                    cellID = Convert.ToInt32(reader["CellID"]);
                    holdStrength = Convert.ToInt32(reader["HoldStrength"]);
                }
                reader.Close();
            };

            await ExecuteQueryAsync(readerProcedure);

            Cell cell = new Cell(cellID, lat, lng, teamID, holdStrength);
            return cell;
        }


        public async static Task<List<Cell>> GetAllCells()
        {
            var cellList = new List<Cell>(256);


            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("GetAllCells", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    decimal lat = (decimal)reader["Latitude"];
                    decimal lng = (decimal)reader["Longitude"];
                    cellList.Add(new Cell(lat, lng));
                }

                reader.Close();
            };

            await ExecuteQueryAsync(readerProcedure);

            return cellList;
        }

        public async static Task UpdateCell(Cell cell)
        {
            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("UpdateCell", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@cellID", SqlDbType.Int).Value = cell.ID;
                cmd.Parameters.Add("@teamID", SqlDbType.Int).Value = cell.TeamID;
                cmd.Parameters.Add("@lat", SqlDbType.Decimal).Value = cell.Latitude;
                cmd.Parameters.Add("@lng", SqlDbType.Decimal).Value = cell.Longitude;
                cmd.Parameters.Add("@holdStrength", SqlDbType.Int).Value = cell.HoldStrength;

                reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {

                }
                reader.Close();
            };

            await ExecuteQueryAsync(readerProcedure);
        }

        public async static void DeletePlayer(int playerID)
        {

            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlCommand cmd = new SqlCommand("DeletePlayer", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@playerID", SqlDbType.Int).Value = playerID;
                await cmd.ExecuteNonQueryAsync();
            };

            await ExecuteQueryAsync(readerProcedure);
        }

        public async static Task DeleteCell(int cellID)
        {
            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlCommand cmd = new SqlCommand("DeleteCell", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@cellID", SqlDbType.Int).Value = cellID;
                await cmd.ExecuteNonQueryAsync();
            };

            await ExecuteQueryAsync(readerProcedure);
        }

        public async static Task UpdatePlayerInfo(Player player, string newUsername, string newPassword)
        {
            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlCommand cmd = new SqlCommand("UpdatePlayer", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@playerID", SqlDbType.Int).Value = player.ID;
                cmd.Parameters.Add("@newUsername", SqlDbType.NVarChar).Value = newUsername;
                cmd.Parameters.Add("@newPassword", SqlDbType.NVarChar).Value = newPassword;
                await cmd.ExecuteNonQueryAsync();
            };

            await ExecuteQueryAsync(readerProcedure);

        }

        public async static Task AddCell(int cellID, decimal lat, decimal lng)
        {
            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("AddCell", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@cellID", SqlDbType.Int).Value = cellID;
                cmd.Parameters.Add("@lat", SqlDbType.Decimal).Value = lat;
                cmd.Parameters.Add("@long", SqlDbType.Decimal).Value = lng;
                reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {

                }
                reader.Close();
            };

            await ExecuteQueryAsync(readerProcedure);
        }

        public async static Task<ConcurrentDictionary<int, Cell>> GetProxyCells(int viewRadius, decimal cellInterval, decimal roundedLat, decimal roundedLng)
        {
            ConcurrentDictionary<int, Cell> cellDict = new ConcurrentDictionary<int, Cell>();

            Func<SqlConnection, Task> readerProcedure = async (SqlConnection connection) =>
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("GetProxyCell", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@viewRadius", SqlDbType.Int).Value = viewRadius;
                cmd.Parameters.Add("@cellInterval", SqlDbType.Decimal).Value = cellInterval;
                cmd.Parameters.Add("@centerCellLat", SqlDbType.Decimal).Value = roundedLat;
                cmd.Parameters.Add("@centerCellLng", SqlDbType.Decimal).Value = roundedLng;
                reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    int cellID = (int)reader["CellID"];
                    decimal lat = (decimal)reader["Latitude"];
                    decimal lng = (decimal)reader["Longitude"];
                    int teamID = (int)reader["TeamID"];
                    int holdStrength = (int)reader["HoldStrength"];
                    Cell cell = new Cell(cellID, lat, lng, teamID, holdStrength);

                    cellDict.TryAdd(cellID, cell);
                }

                reader.Close();
            };

            await ExecuteQueryAsync(readerProcedure);

            return cellDict;
        }

        ///////////////////////////////////////////|||||||||||||||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        ////////////////////////////////////////// OLDE IMPLEMENTS \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        //////////////////////////////////////////|||||||||||||||||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

        //public static int AddUser(string username, string password, int teamID)
        //{
        //    if (!initialized) Init_();
        //    int available = 0;
        //    using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("AddPlayer", connection))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
        //            cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;
        //            cmd.Parameters.Add("@teamID", SqlDbType.Int).Value = teamID;

        //            try
        //            {
        //                SqlDataReader reader;
        //                cmd.Connection.Open();
        //                reader = cmd.ExecuteReader();
        //                reader.Read();
        //                available = (int)reader["Available"];
        //                reader.Close();
        //                cmd.Connection.Close();
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine(e.ToString());
        //                return 2;
        //            }
        //        }
        //    }
        //    return available;
        //}

        //public static int ValidateLoginCredentials(string username, string password)
        //{
        //    if (!initialized) Init_();

        //    // userValidity 1 if valid, 0 if invalid
        //    int userValidity = 0;

        //    using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("LookUpUsername", connection))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
        //            cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;

        //            try
        //            {
        //                SqlDataReader reader;
        //                cmd.Connection.Open();

        //                reader = cmd.ExecuteReader();
        //                reader.Read();
        //                userValidity = (int)reader["Valid"];
        //                reader.Close();
        //                cmd.Connection.Close();
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine(e.ToString());
        //            }
        //        }
        //    }

        //    return userValidity;
        //}
    }
}
