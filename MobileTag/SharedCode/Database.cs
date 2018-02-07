using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Data.SqlClient;
using System.Data;
using MobileTag.Models;
using Android.Graphics;

namespace MobileTag
{
    public static class Database
    {
        private static string CONNECTION_STRING = "";
        private static bool initialized = false;
        private delegate void Del(SqlConnection connection);

        private static void Init_()
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

        public static int AddUser(string username, string password, int teamID)
        {
            int available = 0;

            Del readerProcedure = delegate(SqlConnection connection)
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
                    cellID = (int)reader["CellID"];
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

            Del readerProcedure = delegate (SqlConnection connection)
            {
                SqlDataReader reader;
                SqlCommand cmd = new SqlCommand("GetCell", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@cellID", SqlDbType.Int).Value = cellID;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lat = (decimal)reader["Latitude"];
                    lng = (decimal)reader["Longitude"];
                }
                reader.Close();
            };

            ExecuteQuery(readerProcedure);

            Cell cell = new Cell(cellID, lat, lng);
            return cell;
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
