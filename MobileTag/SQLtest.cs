using System;
using System.Data.SqlClient;
using System.Text;

public static class SQLtest
{
    
    static void TestInsert(string username, string password, int teamID)
    {
        try
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "mobiletag.database.windows.net";
            builder.UserID = "eallgood";
            builder.Password = "orangeChicken17";
            builder.InitialCatalog = "MobileTagDB";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO Player" +
                          "     ([Username],[Password],[TeamID])" +
                          "VALUES" +
                          "     ('" + username + "', '" + password + "', " + teamID + ");");

                String sqlCmd = sb.ToString();

                using (SqlCommand command = new SqlCommand(sqlCmd, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
                        }
                    }
                }
            }
        }
        catch (SqlExeption e)
        {
            Console.WriteLine(e.ToString());
        }
    }
    
}