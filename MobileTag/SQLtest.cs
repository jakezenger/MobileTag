using System;
using System.Data.SqlClient;
using System.Text;

public class SQLtest
{
    public static void TestInsert(string username, string password, int teamID)
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
                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO Player ([Username], [Password], [TeamID]) ");
                sb.Append(String.Format("VALUES ('{0}','{1}',{2});", username, password, teamID));


                String sqlCmd = sb.ToString();

                using (SqlCommand command = new SqlCommand(sqlCmd, connection))
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                }
            }
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}