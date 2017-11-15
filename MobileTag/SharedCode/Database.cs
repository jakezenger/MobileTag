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

namespace MobileTag
{
    public static class Database
    {
        const string CONNECTION_STRING = "Server = tcpmobiletag.database.windows.net,1433; Initial Catalog = MobileTagDB; Persist Security Info=False;User ID = { eallgood }; Password={orangeChicken17}; MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout = 30;";
        
        public static void AddUser(string username, string password, int teamID)
    {
        using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
        {
            string commandString = String.Format
            (
                "EXECUTE AddUser '{0}', '{1}', { 2};",
                    username, password, teamID
                );

            using (SqlCommand command = new SqlCommand(commandString, connection))
            {
                try
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }

    static int LookUpUser(string username, string password)
    {
        int userValidity = 0;

        using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
        {
            // userValidity 1 if valid, 0 if invalid
                string commandString = String.Format
                (
                    "EXECUTE LookUpUser '{0}', '{1}'",
                    username, password
                );

            using (SqlCommand command = new SqlCommand(commandString, connection))
            {
                try
                {
                    SqlDataReader reader;
                    command.Connection.Open();

                    reader = command.ExecuteReader();
                    reader.Read();
                    userValidity = reader.GetInt32(1);

                    command.Connection.Close();
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
