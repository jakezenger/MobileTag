using System;
using System.Data.SqlClient;
using System.Text;

public class Database
{
    private static SqlConnection GetConnectionString()
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
        builder.DataSource =        "mobiletag.database.windows.net";
        builder.UserID =            "eallgood";
        builder.Password =          "orangeChicken17";
        builder.InitialCatalog =    "MobileTagDB";

        SqlConnection connection = new SqlConnection(builder.ConnectionString);

        return connection;
    }


    //TODO: fill in these methods with SQL functions.
    //Creates new user
    public static bool InsertUser(string username, string password, int TeamID)
    {
        try
        {
            using(SqlConnection connection = GetConnectionString())
            {
                String sqlCommandString = String.Format(
                    "INSERT INTO Player (Username, Password, TeamID)" +
                    "VALUES ('{0}', '{1}', {2});"
                    ,username, password, TeamID
                );

                SqlCommand sqlCommand = new SqlCommand(sqlCommandString, connection);

                connection.Open();
                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
            return true;
        }
        catch(SqlException e)
        {
            Console.WriteLine(e.ToString());
            return false;
        }
    }

    //Validates Login info
    public static bool ValidateCredentials(string username, string password)
    {
        try
        {
            bool hasRows;

            using (SqlConnection connection = GetConnectionString())
            {
                String sqlCommandString = String.Format(
                    "SELECT * FROM Player WHERE Player.Username = '{0}' AND Player.Password = '{1}';"
                    , username, password
                );

                SqlCommand sqlCommand = new SqlCommand(sqlCommandString, connection);

                connection.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                hasRows = reader.HasRows;
                connection.Close();
            }
            return hasRows;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
            return false;
        }
    }

    //Deletes user by username (NOTE: Maybe we should deactivate account instead...)
    public static bool DeleteUser(string username)
    {
        try
        {
            using (SqlConnection connection = GetConnectionString())
            {
                String sqlCommandString = String.Format(
                    "DELETE FROM Users WHERE Player.Username = '{0}';"
                    , username
                );

                SqlCommand sqlCommand = new SqlCommand(sqlCommandString, connection);

                connection.Open();
                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
            return true;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
            return false;
        }
    }

    public static void AddCell(decimal lon, decimal lat)
    {
        try
        {
            using (SqlConnection connection = GetConnectionString())
            {
                String sqlCommandString = String.Format(
                    "INSERT INTO Cell (Longitude, Latitude)" +
                    "VALUES ({0}, {1});"
                    , lon, lat
                );

                SqlCommand sqlCommand = new SqlCommand(sqlCommandString, connection);

                connection.Open();
                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static void UpdateCell(int CellID, int TeamID)
    {
        try
        {
            using (SqlConnection connection = GetConnectionString())
            {
                String sqlCommandString = String.Format(
                    "UPDATE Cell" +
                    "SET Cell.TeamID = {1}" +
                    "WHERE Cell.CellID = {0};"
                    , CellID, TeamID
                );

                SqlCommand sqlCommand = new SqlCommand(sqlCommandString, connection);

                connection.Open();
                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    //WARNING: THIS DELETES THE ENTIRE CELL TABLE
    public static void InitCells(decimal startLong, decimal startLat, decimal endLong, decimal endLat, decimal increment)
    {
        try
        {
            using (SqlConnection connection = GetConnectionString())
            {
                String sqlCommandString = String.Format(
                    "DELETE FROM Cells;"
                );

                SqlCommand sqlCommand = new SqlCommand(sqlCommandString, connection);

                connection.Open();
                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
        }

        int i = 0;
        int j = 0;

        while (endLat > (startLat + increment * i))
        {
            while (endLong > (startLong + increment * j))
            {
                AddCell(startLong + increment * j, startLat + increment * i);
                i++;
            }
            j++;
        }
    }
}