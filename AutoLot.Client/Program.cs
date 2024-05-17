using AutoLot.DAL.DataOperations;
using AutoLot.DAL.Models;

namespace AutoLot.Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            //InventoryDal dal = new InventoryDal();

            //List<CarViewModel> list = dal.GetAllInventory();

            //Console.WriteLine("*** ALL CARS ***");
            //Console.WriteLine("Id\tMake\tColor\tPet Name");
            //foreach (CarViewModel item in list)
            //{
            //    Console.WriteLine($"{item.Id}\t{item.Make}\t{item.Color}\t{item.PetName}");
            //}
            //Console.WriteLine();

            //CarViewModel car = dal.GetCar(list.OrderBy(x => x.Color).Select(x => x.Id).First());

            //Console.WriteLine("*** First car by color ***");
            //Console.WriteLine("CarId\tMake\tColor\tPet Name");
            //Console.WriteLine($"{car.Id}\t{car.Make}\t{car.Color}\t{car.PetName}");
            //Console.WriteLine();

            //try
            //{
            //    dal.DeleteCar(5);
            //    Console.WriteLine("Car deleted");
            //}

            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Exeption handled: {ex.Message}");
            //}

            //Console.WriteLine();

            //dal.InsertAuto(new Car { Color = "Blue", MakeId = 5, PetName = "TowMonster" });
            //list = dal.GetAllInventory();
            //var newCar = list.First(x => x.PetName == "TowMonster");

            //Console.WriteLine("*** New car ***");
            //Console.WriteLine("CarId\tMake\tColor\tPet Name");
            //Console.WriteLine($"{newCar.Id}\t{newCar.Make}\t{newCar.Color}\t{newCar.PetName}");
            //Console.WriteLine();

            //dal.DeleteCar(newCar.Id);
            //var petName = dal.LookUpPetName(car.Id);

            //Console.WriteLine("*** Name of first car by color ***");
            //Console.WriteLine($"Car Pet Name: {petName}");
            //Console.WriteLine("Press enter to continue...");
            FlagCustomer();
            Console.ReadKey();
        }

        static void FlagCustomer()
        {
            Console.WriteLine("*** Simple transaction example ***");

            bool throwEx = true;
            Console.Write("Do you want to throw an exception (Y or N): ");
            var userAnswer = Console.ReadLine();
            if (string.IsNullOrEmpty(userAnswer) || userAnswer.Equals("N", StringComparison.OrdinalIgnoreCase))
            {
                throwEx = false;
            }

            var dal = new InventoryDal();
            dal.ProcessCreditRisk(throwEx, 1);

            Console.WriteLine("Check credit risks tale for result");
            Console.ReadLine();
        }
    }
}