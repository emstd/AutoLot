using Microsoft.Data.SqlClient;

namespace AutoLot.DataReader
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = "Server=(localdb)\\mssqllocaldb;database=AutoLot;trusted_connection=true;TrustServerCertificate=True";
                connection.Open();
                string sql = @"Select i.id, m.Name as Make, i.Color, i.Petname
                                FROM Inventory i
                               INNER JOIN Makes m on m.Id = i.MakeId";
                SqlCommand command = new SqlCommand(sql, connection);
                using (SqlDataReader myDataReader = command.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        //Console.WriteLine($"-> Make: {myDataReader["Make"]}, PetName: {myDataReader["PetName"]}, Color: {myDataReader["Color"]}");
                        Console.WriteLine(myDataReader.GetString);
                    }
                }
            }
        }
    }
}