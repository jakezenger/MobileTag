using System;

using System.Data.SqlClient;
using System.Data;
using MobileTag.Models;
using System.Collections.Generic;
using System.Collections.Concurrent;

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
            CONNECTION_STRING = builder.ConnectionString.ToString();
            initialized = true;
        }

        private static void ExecuteQuery(Del del)
        {
            if (!initialized) Init_();
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                try
                {
                    connection.Open();
                    del(connection);
                    connection.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        // can we use player instead of user?  
        public static int AddUser(string username, string password, int teamID)
        {
            int available = 0;

            Del readerProcedure = delegate (SqlConnection connection)
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("AddPlayer", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;
                cmd.Parameters.Add("@teamID", SqlDbType.Int).Value = teamID;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    available = (int)reader["Available"];
                }
                reader.Close();
            };

            ExecuteQuery(readerProcedure);

            return available;
        }

        public static int ValidateLoginCredentials(string username, string password)
        {
            int userValidity = 0;

            Del readerProcedure = delegate (SqlConnection connection)
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("LookUpUserName", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    userValidity = (int)reader["Valid"];
                }
                reader.Close();
            };

            ExecuteQuery(readerProcedure);

            return userValidity;
        }

        public static int GetCellTeam(int cellID)
        {
            int teamID = 0;

            Del readerProcedure = delegate (SqlConnection connection)
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("GetCellTeam", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@cellID", SqlDbType.Int).Value = cellID;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    teamID = (int)reader["TeamID"];
                }
                reader.Close();
            };

            ExecuteQuery(readerProcedure);

            return teamID;
        }

        public static Player GetPlayer(string username)
        {
            int playerID = 0;
            int teamID = 0;
            string teamName = "";

            int cellID = 0;

            Del readerProcedure = delegate (SqlConnection connection)
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("GetPlayer", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    playerID = (int)reader["PlayerID"];
                    teamID = (int)reader["TeamID"];
                    teamName = (string)reader["TeamName"];
                    cellID = 0; //(int)reader["CellID"];
                }
                reader.Close();
            };

            ExecuteQuery(readerProcedure);

            Team team = new Team(teamID, teamName);
            Player player = new Player(playerID, username, team, cellID);
            return player;
        }

        public static Cell GetCell(int cellID)
        {
            decimal lat = 0.00m;
            decimal lng = 0.00m;
            int teamID = 0;

            Del readerProcedure = delegate (SqlConnection connection)
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("GetCell", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@cellID", SqlDbType.Int).Value = cellID;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lat = Convert.ToDecimal(reader["Latitude"]);
                    lng = Convert.ToDecimal(reader["Longitude"]);
                    teamID = Convert.ToInt32(reader["TeamID"]);
                    cellID = Convert.ToInt32(reader["CellID"]);
                }
                reader.Close();
            };

            ExecuteQuery(readerProcedure);

            Cell cell = new Cell(cellID, lat, lng, teamID);
            return cell;
        }


        public static List<Cell> GetAllCells()
        {
            var cellList = new List<Cell>(256);


            Del readerProcedure = delegate (SqlConnection connection)
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("GetAllCells", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    decimal lat = (decimal)reader["Latitude"];
                    decimal lng = (decimal)reader["Longitude"];
                    cellList.Add(new Cell(lat, lng));
                }

                reader.Close();
            };

            ExecuteQuery(readerProcedure);

            return cellList;
        }

        public static void UpdateCell(Cell cell, int teamID)
        {
            Del readerProcedure = delegate (SqlConnection connection)
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("UpdateCell", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@cellID", SqlDbType.Int).Value = cell.ID;
                cmd.Parameters.Add("@teamID", SqlDbType.Int).Value = teamID;
                cmd.Parameters.Add("@lat", SqlDbType.Decimal).Value = cell.Latitude;
                cmd.Parameters.Add("@lng", SqlDbType.Decimal).Value = cell.Longitude;

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                }
                reader.Close();
            };

            ExecuteQuery(readerProcedure);
        }

        public static void DeletePlayer(int playerID)
        {

            Del readerProcedure = delegate (SqlConnection connection)
            {
                SqlCommand cmd = new SqlCommand("DeletePlayer", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@playerID", SqlDbType.Int).Value = playerID;
                cmd.ExecuteNonQuery();
            };

            ExecuteQuery(readerProcedure);
        }

        public static void DeleteCell(int cellID)
        {

            Del readerProcedure = delegate (SqlConnection connection)
            {
                SqlCommand cmd = new SqlCommand("DeleteCell", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@cellID", SqlDbType.Int).Value = cellID;
                cmd.ExecuteNonQuery();
            };

            ExecuteQuery(readerProcedure);
        }

        public static void UpdatePlayerInfo(Player player, string newUsername, string newPassword)
        {
            Del readerProcedure = delegate (SqlConnection connection)
            {
                SqlCommand cmd = new SqlCommand("UpdatePlayer", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@playerID", SqlDbType.Int).Value = player.ID;
                cmd.Parameters.Add("@newUsername", SqlDbType.NVarChar).Value = newUsername;
                cmd.Parameters.Add("@newPassword", SqlDbType.NVarChar).Value = newPassword;
                cmd.ExecuteNonQuery();
            };

            ExecuteQuery(readerProcedure);

        }

        public static void AddCell(int cellID, decimal lat, decimal lng)
        {
            Del readerProcedure = delegate (SqlConnection connection)
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("AddCell", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@cellID", SqlDbType.Int).Value = cellID;
                cmd.Parameters.Add("@lat", SqlDbType.Decimal).Value = lat;
                cmd.Parameters.Add("@long", SqlDbType.Decimal).Value = lng;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                }
                reader.Close();
            };

            ExecuteQuery(readerProcedure);
        }

        public static ConcurrentDictionary<int, Cell> GetProxyCells(int viewRadius, decimal cellInterval, decimal roundedLat, decimal roundedLng)
        {
            ConcurrentDictionary<int, Cell> cellDict = new ConcurrentDictionary<int, Cell>();

            Del readerProcedure = delegate (SqlConnection connection)
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("GetProxyCell", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@viewRadius", SqlDbType.Int).Value = viewRadius;
                cmd.Parameters.Add("@cellInterval", SqlDbType.Decimal).Value = cellInterval;
                cmd.Parameters.Add("@centerCellLat", SqlDbType.Decimal).Value = roundedLat;
                cmd.Parameters.Add("@centerCellLng", SqlDbType.Decimal).Value = roundedLng;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int cellID = (int)reader["CellID"];
                    decimal lat = (decimal)reader["Latitude"];
                    decimal lng = (decimal)reader["Longitude"];
                    int teamID = (int)reader["TeamID"];
                    Cell cell = new Cell(cellID, lat, lng, teamID);

                    cellDict.TryAdd(cellID, cell);
                }
                reader.Close();
            };

            ExecuteQuery(readerProcedure);

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
