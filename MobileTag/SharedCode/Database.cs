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

namespace MobileTag
{
    public static class Database
    {
        private static string CONNECTION_STRING = "";
        private static bool initialized = false;

        public static void Init_()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "mobiletag.database.windows.net";
            builder.UserID = "eallgood";
            builder.Password = "orangeChicken17";
            builder.InitialCatalog = "MobileTagDB";

            CONNECTION_STRING = builder.ConnectionString.ToString();
            initialized = true;
        }

        public static void AddUser(string username, string password, int teamID)
        {
            if (!initialized) Init_();
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                using (SqlCommand cmd = new SqlCommand("AddPlayer", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                    cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;
                    cmd.Parameters.Add("@teamID", SqlDbType.Int).Value = teamID;

                    try
                    {
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                    catch (SqlException e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }

        static int ValidateLoginCredentials(string username, string password)
        {
            if (!initialized) Init_();

            // userValidity 1 if valid, 0 if invalid
            int userValidity = 0;

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {

                using (SqlCommand cmd = new SqlCommand("LookUpUsername", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                    cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;

                    try
                    {
                        SqlDataReader reader;
                        cmd.Connection.Open();

                        reader = cmd.ExecuteReader();
                        reader.Read();
                        userValidity = reader.GetInt32(1);

                        cmd.Connection.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }

            return userValidity;
        }
    }  
}
