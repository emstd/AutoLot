using AutoLot.DAL.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AutoLot.DAL.DataOperations
{
    public class InventoryDal : IDisposable
    {
        private readonly string _connectionString;

        public InventoryDal() : this("Server=(localdb)\\mssqllocaldb;database=AutoLot;trusted_connection=true;TrustServerCertificate=True")
        {
        }

        public InventoryDal(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqlConnection _sqlConnection = null;

        private void OpenConnection()
        {
            _sqlConnection = new SqlConnection(_connectionString);
            _sqlConnection.Open();
        }

        private void CloseConnection()
        {
            if (_sqlConnection?.State != ConnectionState.Closed)
            {
                _sqlConnection?.Close();
            }
        }


        bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                _sqlConnection.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }




        public List<CarViewModel> GetAllInventory()
        {
            OpenConnection();
            List<CarViewModel> inventory = new();

            string sql = @"SELECT i.Id, i.Color, i.PetName, m.Name as Make
                             FROM Inventory i
                               INNER JOIN Makes m on m.Id = i.MakeId";

            using SqlCommand command = new(sql, _sqlConnection)
            {
                CommandType = CommandType.Text
            };

            SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
            while (dataReader.Read())
            {
                inventory.Add(new CarViewModel()
                {
                    Id = (int)dataReader["Id"],
                    Color = (string)dataReader["Color"],
                    Make = (string)dataReader["Make"],
                    PetName = (string)dataReader["PetName"]
                });
            }
            dataReader.Close();
            return inventory;
        }

        public CarViewModel GetCar(int id)
        {
            SqlParameter param = new SqlParameter()
            {
                ParameterName = "@carId",
                Value = id,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            OpenConnection();
            CarViewModel car = null;
            string sql = @"SELECT i.Id, i.Color, i.PetName, m.Name as Make
                            FROM Inventory i
                            INNER JOIN Makes m on i.MakeId = m.Id
                            WHERE i.Id = @carId";
            using SqlCommand command = new SqlCommand(sql, _sqlConnection)
            {
                CommandType = CommandType.Text
            };
            command.Parameters.Add(param);

            SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
            while (dataReader.Read())
            {
                car = new CarViewModel()
                {
                    Id = dataReader.GetInt32("id"),
                    Color = dataReader.GetString("color"),
                    Make = dataReader.GetString("make"),
                    PetName = dataReader.GetString("petname")
                };
            }

            dataReader.Close();
            return car;
        }

        public void InsertAuto(string color, int makeId, string petName)
        {
            OpenConnection();
            string sql = $"INSERT INTO Inventory (MakeId, Color, PetName) VALUES ('{makeId}', '{color}', '{petName}')";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            CloseConnection();
        }

        public void InsertAuto(Car car)
        {
            OpenConnection();
            string sql = $"INSERT INTO Inventory (MakeId, Color, PetName) VALUES (@MakeId, @Color, @PetName)";
            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                SqlParameter parameter = new()
                {
                    ParameterName = "@MakeId",
                    Value = car.MakeId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                };
                command.Parameters.Add(parameter);

                parameter = new SqlParameter()
                {
                    ParameterName = "@Color",
                    Value = car.Color,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 50,
                    Direction = ParameterDirection.Input
                };
                command.Parameters.Add(parameter);

                parameter = new SqlParameter()
                {
                    ParameterName = @"PetName",
                    Value = car.PetName,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 50,
                    Direction = ParameterDirection.Input
                };
                command.Parameters.Add(parameter);

                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            CloseConnection();
        }

        public void DeleteCar(int id)
        {
            SqlParameter param = new()
            {
                ParameterName = "@carId",
                Value = id,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };
            OpenConnection();
            string sql = $"DELETE FROM Inventory WHERE Id = @carId";
            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.Parameters.Add(param);
                try
                {
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Exception error = new Exception("Sorry, this car is on order", ex);
                    throw error;
                }
            }
            CloseConnection();
        }

        public void UpdateCarPetName(int id, string newPetName)
        {
            SqlParameter paramId = new()
            {
                ParameterName = "@carId",
                Value = id,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };
            SqlParameter paramName = new()
            {
                ParameterName = "@petName",
                Value = newPetName,
                SqlDbType = SqlDbType.NVarChar,
                Size = 50,
                Direction = ParameterDirection.Input
            };

            OpenConnection();
            string sql = $"UPDATE Inventory SET PetName = @petName WHERE Id = @carId";
            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.Parameters.Add(paramId);
                command.Parameters.Add(paramName);
                command.ExecuteNonQuery();
            }
            CloseConnection();
        }

        public string LookUpPetName(int carId)
        {
            OpenConnection();
            string carPetName;

            using (SqlCommand command = new SqlCommand("GetPetName", _sqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter()
                {
                    ParameterName = "@carId",
                    Value = carId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                };
                command.Parameters.Add(param);

                param = new SqlParameter()
                {
                    ParameterName = "@petName",
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 50,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(param);

                command.ExecuteNonQuery();

                carPetName = (string)command.Parameters["@petName"].Value;
                CloseConnection();
            }

            return carPetName;
        }

        public void ProcessCreditRisk(bool throwEx, int customerId)
        {
            OpenConnection();
            string fName;
            string lName;
            using var cmdSelect = new SqlCommand("SELECT * FROM Customers WHERE Id = @customerId", _sqlConnection);
            SqlParameter paramId = new SqlParameter()
            {
                ParameterName = "@customerId",
                SqlDbType = SqlDbType.Int,
                Value = customerId,
                Direction = ParameterDirection.Input
            };
            cmdSelect.Parameters.Add(paramId);

            using (var dataReader = cmdSelect.ExecuteReader())
            {
                if (dataReader.HasRows)
                {
                    dataReader.Read();
                    fName = (string)dataReader["FirstName"];
                    lName = (string)dataReader["LastName"];
                }
                else
                {
                    CloseConnection();
                    return;
                }
            }
            cmdSelect.Parameters.Clear();

            using var cmdUpdate = new SqlCommand("UPDATE Customers SET LastName = LastName + '(CreditRisk)' WHERE Id = @customerId", _sqlConnection);
            cmdUpdate.Parameters.Add(paramId);

            using var cmdInsert = new SqlCommand("INSERT INTO CreditRisks (CustomerId, FirstName, LastName) VALUES (@CustomerId, @FirstName, @LastName)", _sqlConnection);
            SqlParameter parameterId = new()
            {
                ParameterName = "@CustomerId",
                Value = customerId,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };
            SqlParameter parameterFirstName = new()
            {
                ParameterName = "@FirstName",
                Value = fName,
                SqlDbType = SqlDbType.NVarChar,
                Size = 50,
                Direction = ParameterDirection.Input
            };
            SqlParameter parameterLastName = new()
            {
                ParameterName = "@LastName",
                Value = lName,
                SqlDbType = SqlDbType.NVarChar,
                Size = 50,
                Direction = ParameterDirection.Input
            };
            cmdInsert.Parameters.Add(parameterId);
            cmdInsert.Parameters.Add(parameterFirstName);
            cmdInsert.Parameters.Add(parameterLastName);

            SqlTransaction tx = null;
            try
            {
                tx = _sqlConnection.BeginTransaction();
                cmdInsert.Transaction = tx;
                cmdUpdate.Transaction = tx;

                cmdInsert.ExecuteNonQuery();
                cmdUpdate.ExecuteNonQuery();

                if (throwEx)
                {
                    throw new Exception("DataBase ERROR!");
                }
                tx.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                tx?.Rollback();
            }
            finally
            {
                _sqlConnection.Close();
                tx.Dispose();
            }
        }
    }
}
