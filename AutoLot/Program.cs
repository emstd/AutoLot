using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace AutoLot
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*** DataProvider ***");
            var (provider, connectionString) = GetProviderFromConfiguration();
            DbProviderFactory? factory = GetDbProviderFactory(provider);

            using(DbConnection connection = factory.CreateConnection())
            {
                if (connection == null)
                {
                    Console.WriteLine("Не удалось установить подключение");
                    return;
                }
                Console.WriteLine($"Объект Connection GetType(): {connection.GetType()}");
                Console.WriteLine($"Объект Connection GetType().Name: {connection.GetType().Name}");
                Console.WriteLine();

                connection.ConnectionString = connectionString;
                connection.Open();

                DbCommand? command = factory.CreateCommand();

                if (command == null)
                {
                    Console.WriteLine("Не удалось создать объект команды");
                }

                Console.WriteLine($"Объект Command GetType(): {command.GetType()}");
                Console.WriteLine($"Объект Commannd GetType().Name: {command.GetType().Name}");
                Console.WriteLine();

                command.Connection = connection;
                command.CommandText = "select i.Id, m.Name from Inventory i inner join Makes m on m.Id = i.MakeId";

                using(DbDataReader dataReader = command.ExecuteReader())
                {
                    Console.WriteLine($"DataReader object GetType(): {dataReader.GetType()}");
                    Console.WriteLine($"DataReader GetType().Name: {dataReader.GetType().Name}");
                    Console.WriteLine("Объекты из базы: ");
                    while(dataReader.Read())
                    {
                        Console.WriteLine($"Car {dataReader[0]} is a {dataReader["Name"]}");
                    }
                }
            }
        }

        static DbProviderFactory? GetDbProviderFactory(DataProviderEnum provider)
        {
            if (provider == DataProviderEnum.SqlServer)
            {
                return SqlClientFactory.Instance;
            }
            return null;
        }

        static (DataProviderEnum provider, string? ConnectionString) GetProviderFromConfiguration()
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            var providerName = config["ProviderName"];
            if (Enum.TryParse<DataProviderEnum>(providerName, out DataProviderEnum provider))
            {
                return (provider, config[$"{providerName}:ConnectionString"]);
            }
            throw new Exception();
        }
    }

}