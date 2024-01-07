using homework_2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Xml.Serialization;

namespace homework_2
{

    // vehicle class
    public abstract class Vehicle
    {
        public DateTime currentDate = DateTime.Now;
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int YearOfManufacture { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public string RegistrationNumber { get; set; }
        public int Mileage { get; set; }
        public abstract decimal CalculateRentalCost(double duration, double distance);
        public abstract bool NeedsMaintenance();
    }

    // Pessenger vehicle class  
    public class PassengerVehicle : Vehicle
    {
        const int maintenancethreshold = 5000;
        public double Depreciation = 0.9;
        public double SpecificCarModelCoefficient { get; set; }
        public int LesseeRating { get; set; }

        // calculates rental cost of passenger vehicles
        public override decimal CalculateRentalCost(double duration, double distance)
        {
            return (decimal)(duration * distance * SpecificCarModelCoefficient * LesseeRating);
        }

        // maintenance threshold for passenger vehicles
        public override bool NeedsMaintenance()
        {
            int MaxDistanceTotal = maintenancethreshold * (currentDate.Year - YearOfManufacture);
            return MaxDistanceTotal - Mileage <= 1000;
        }
    }

    // Cargo vehicle class
    public class CargoVehicle : Vehicle
    {
        const int maintenancethreshold = 15000;
        public double Depreciation = 0.93;
        public double ModelCoefficient { get; set; }
        public double CargoWeight { get; set; }

        // calculates rental cost of cargo vehicles
        public override decimal CalculateRentalCost(double duration, double distance)
        {
            return (decimal)(duration * distance * ModelCoefficient * CargoWeight);
        }

        // maintenance threshold for cargo vehicles
        public override bool NeedsMaintenance()
        {
            int MaxDistanceTotal = maintenancethreshold * (currentDate.Year - YearOfManufacture);
            return MaxDistanceTotal - Mileage <= 1000;
        }
    }

    // container class
    public class VehicleFleet
    {
        public int BrandQuantity = 0;
        public int ColorQuantity = 0;

        public string[] BrandArray = { };
        public string[] ColorArray = { };

        private List<Vehicle> vehicles = new List<Vehicle>();

        public void AddVehicle(Vehicle vehicle)
        {
            vehicles.Add(vehicle);
        }

        public List<Vehicle> ListVehiclesByBrand(string brand)
        {
            return vehicles.Where(v => v.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<Vehicle> ListVehiclesExceedingTenure(string model)
        {
            DateTime currentDate = DateTime.Now;
            return vehicles.Where(v =>
            {
                if (v.Model.Equals(model, StringComparison.OrdinalIgnoreCase))
                {
                    if (v is PassengerVehicle passengervehicle)
                    {
                        return currentDate.Year - passengervehicle.YearOfManufacture >= 5 ||
                           passengervehicle.NeedsMaintenance();
                    }
                    else if (v is CargoVehicle cargovehicle)
                    {
                        return currentDate.Year - cargovehicle.YearOfManufacture >= 15 ||
                           cargovehicle.NeedsMaintenance();
                    }
                }
                return false;

            }).ToList();
        }

        public decimal CalculateTotalValueOfFleet()
        {
            DateTime currentDate = DateTime.Now;
            return vehicles.Sum(v =>
            {
                if (v is PassengerVehicle passengerVehicle)
                {
                    return passengerVehicle.Price * (decimal)Math.Pow(passengerVehicle.Depreciation, currentDate.Year - passengerVehicle.YearOfManufacture);
                }
                else if (v is CargoVehicle cargoVehicle)
                {
                    return cargoVehicle.Price * (decimal)Math.Pow(cargoVehicle.Depreciation, currentDate.Year - cargoVehicle.YearOfManufacture);
                }
                return 0;
            });
        }

        public List<Vehicle> ListVehiclesByPreference(string brand, string color)
        {
            return vehicles.Where(v => v.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase)
                                    && v.Color.Equals(color, StringComparison.OrdinalIgnoreCase))
                          .OrderByDescending(v =>
                          {
                              if (v is PassengerVehicle passengerVehicle)
                              {
                                  return passengerVehicle.LesseeRating;
                              }
                              else if (v is CargoVehicle cargoVehicle)
                              {
                                  return cargoVehicle.CargoWeight;
                              }
                              return 0;
                          })
                          .ToList();
        }

        public List<Vehicle> ListVehiclesRequiringMaintenance()
        {
            return vehicles.Where(v =>
            {
                if (v is PassengerVehicle passengerVehicle)
                {
                    return passengerVehicle.NeedsMaintenance();
                }
                else if (v is CargoVehicle cargoVehicle)
                {
                    return cargoVehicle.NeedsMaintenance();
                }
                return false;
            }).ToList();
        }
        public void ReadData(string jsonfile)
        {
            String jsonString = new StreamReader(jsonfile).ReadToEnd();

            // Create a JsonNode DOM from a JSON string.
            JsonNode carsNode = JsonNode.Parse(jsonString);

            JsonArray carsArray = carsNode.AsArray();
            for (int i = 0; i < carsArray.Count; i++)
            {
                if (!BrandArray.Contains(carsArray[i]["Brand"].ToString())) // if is not
                {
                    BrandArray = BrandArray.Append(carsArray[i]["Brand"].ToString()).ToArray();
                    BrandQuantity++;
                }

                if (!ColorArray.Contains(carsArray[i]["Color"].ToString())) // if is not
                {
                    ColorArray = ColorArray.Append(carsArray[i]["Color"].ToString()).ToArray();
                    ColorQuantity++;
                }

                if (carsArray[i].ToString().Contains("LesseeRating"))  //passenger car
                {
                    AddVehicle(new PassengerVehicle
                    {
                        Id = (int)carsArray[i]["Id"],
                        Brand = carsArray[i]["Brand"].ToString(),
                        Model = carsArray[i]["Model"].ToString(),
                        YearOfManufacture = (int)carsArray[i]["YearOfManufacture"],
                        Color = carsArray[i]["Color"].ToString(),
                        Price = (decimal)carsArray[i]["Price"],
                        RegistrationNumber = carsArray[i]["RegistrationNumber"].ToString(),
                        Mileage = (int)carsArray[i]["Mileage"],
                        SpecificCarModelCoefficient = (double)carsArray[i]["SpecificCarModelCoefficient"],
                        LesseeRating = (int)carsArray[i]["LesseeRating"]
                    });
                }
                else
                {
                    AddVehicle(new CargoVehicle
                    {
                        Id = (int)carsArray[i]["Id"],
                        Brand = carsArray[i]["Brand"].ToString(),
                        Model = carsArray[i]["Model"].ToString(),
                        YearOfManufacture = (int)carsArray[i]["YearOfManufacture"],
                        Color = carsArray[i]["Color"].ToString(),
                        Price = (decimal)carsArray[i]["Price"],
                        RegistrationNumber = carsArray[i]["RegistrationNumber"].ToString(),
                        Mileage = (int)carsArray[i]["Mileage"],
                        ModelCoefficient = (double)carsArray[i]["ModelCoefficient"],
                        CargoWeight = (double)carsArray[i]["CargoWeight"]
                    });
                }
            }
        }
    }

    public class Menu
    {
        public int choice, choice2, choice3;
        JsonArray menuArray;
        public void DisplayMenu()
        {
            String jsonString = new StreamReader("menu.json").ReadToEnd();
            JsonNode menuNode = JsonNode.Parse(jsonString);
            menuArray = menuNode.AsArray();

            Console.Clear();   // clear the screen
            Console.WriteLine("Company X, fleet management system:\n\n");
            for (int i = 0; i < menuArray.Count(); i++)
            {
                Console.WriteLine(i + ". " + menuArray[i]);
            }
            Console.Write("Choose one from above:");
        }

        public void SelectMenu(VehicleFleet fleet)
        {
            choice = ChooseItem(0, menuArray.Count);
            switch (choice)
            {
                case 1:       // a)
                    Console.Write("Select ");
                    for (int i = 0; i < fleet.BrandQuantity; i++)
                    {
                        Console.Write(i + 1);
                        Console.Write("-" + fleet.BrandArray[i] + "  ");
                    }
                    Console.Write(":");
                    choice2 = ChooseItem(1, fleet.BrandQuantity+1);
                    var vehiclesByBrand = fleet.ListVehiclesByBrand(fleet.BrandArray[choice2 - 1]);
                    Console.WriteLine("\nCount of " + fleet.BrandArray[choice2 - 1] + "'s: " + vehiclesByBrand.Count);
                    Console.WriteLine("Id  Brand       Model       RegistNum   Color    YearOfManuf  Price     Mileage");
                    for (int i = 0; i < vehiclesByBrand.Count; i++)
                    {
                        Console.Write("{0,2}  {1,-12}{2,-12}{3,-12}", vehiclesByBrand[i].Id, vehiclesByBrand[i].Brand, vehiclesByBrand[i].Model, vehiclesByBrand[i].RegistrationNumber);
                        Console.Write("{0,-9}{1,-13}{2,-10}{3,8}\n", vehiclesByBrand[i].Color, vehiclesByBrand[i].YearOfManufacture, vehiclesByBrand[i].Price, vehiclesByBrand[i].Mileage);
                    }
                    Console.Write("\nTo come back to the main menu press enter.");
                    Console.ReadLine();
                    break;

                case 2:
                    Console.Write("\nCount of vehicles with exceeding tenure:\n");
                    var vehiclesExceedingTenure = fleet.ListVehiclesExceedingTenure("Transit");
                    Console.WriteLine("Id  Brand       Model       RegistNum   Color    YearOfManuf  Price     Mileage");
                    for (int i = 0; i < vehiclesExceedingTenure.Count; i++)
                    {
                        Console.Write("{0,2}  {1,-12}{2,-12}{3,-12}", vehiclesExceedingTenure[i].Id, vehiclesExceedingTenure[i].Brand, vehiclesExceedingTenure[i].Model, vehiclesExceedingTenure[i].RegistrationNumber);
                        Console.Write("{0,-9}{1,-13}{2,-10}{3,8}\n", vehiclesExceedingTenure[i].Color, vehiclesExceedingTenure[i].YearOfManufacture, vehiclesExceedingTenure[i].Price, vehiclesExceedingTenure[i].Mileage);
                    }
                    Console.Write("\nTo come back to the main menu press enter.");
                    Console.ReadLine();
                    break;

                case 3:
                    var totalFleetValue = fleet.CalculateTotalValueOfFleet();
                    Console.WriteLine("\nTotal fleet value: " + totalFleetValue.ToString("0.00"));
                    Console.Write("\nTo come back to the main menu press enter.");
                    Console.ReadLine();
                    break;

                case 4:

                    Console.Write("Select ");
                    for (int i = 0; i < fleet.BrandQuantity; i++)
                    {
                        Console.Write(i + 1);
                        Console.Write("-" + fleet.BrandArray[i] + "  ");
                    }
                    Console.Write(":");
                    choice2 = ChooseItem(1, fleet.BrandQuantity + 1);
                    Console.Write("\nSelect ");
                    for (int i = 0; i < fleet.ColorQuantity; i++)
                    {
                        Console.Write(i + 1);
                        Console.Write("-" + fleet.ColorArray[i] + "  ");
                    }
                    Console.Write(":");
                    choice3 = ChooseItem(1, fleet.ColorQuantity + 1);

                    // Querying the fleet
                    List<Vehicle> vehiclesByPreference = fleet.ListVehiclesByPreference(fleet.BrandArray[choice2 - 1], fleet.ColorArray[choice3 - 1]);
                    Console.WriteLine("\nCount of vehicles with preferences: " + vehiclesByPreference.Count);
                    Console.WriteLine("Id  Brand       Model       RegistNum   Color    YearOfManuf  Price     Mileage");
                    for (int i = 0; i < vehiclesByPreference.Count; i++)
                    {
                        Console.Write("{0,2}  {1,-12}{2,-12}{3,-12}", vehiclesByPreference[i].Id, vehiclesByPreference[i].Brand, vehiclesByPreference[i].Model, vehiclesByPreference[i].RegistrationNumber);
                        Console.Write("{0,-9}{1,-13}{2,-10}{3,8}\n", vehiclesByPreference[i].Color, vehiclesByPreference[i].YearOfManufacture, vehiclesByPreference[i].Price, vehiclesByPreference[i].Mileage);
                    }
                    Console.Write("\nTo come back to the main menu press enter.");
                    Console.ReadLine();
                    break;
                case 5:
                    var vehiclesRequiringMaintenance = fleet.ListVehiclesRequiringMaintenance();
                    Console.WriteLine("\nCount of vehicles needing maintenance: " + vehiclesRequiringMaintenance.Count);
                    Console.WriteLine("Id  Brand       Model       RegistNum   Color    YearOfManuf  Price     Mileage");
                    for (int i = 0; i < vehiclesRequiringMaintenance.Count; i++)
                    {
                        Console.Write("{0,2}  {1,-12}{2,-12}{3,-12}", vehiclesRequiringMaintenance[i].Id, vehiclesRequiringMaintenance[i].Brand, vehiclesRequiringMaintenance[i].Model, vehiclesRequiringMaintenance[i].RegistrationNumber);
                        Console.Write("{0,-9}{1,-13}{2,-10}{3,8}\n", vehiclesRequiringMaintenance[i].Color, vehiclesRequiringMaintenance[i].YearOfManufacture, vehiclesRequiringMaintenance[i].Price, vehiclesRequiringMaintenance[i].Mileage);
                    }
                    Console.Write("\nTo come back to the main menu press enter.");
                    Console.ReadLine();
                    break;
            }
        }
        public int ChooseItem(int StartNo, int LastNo)
        {
            do
            {
                try
                {
                    choice = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.Write("Invalid input, try again:");
                    choice = -1;
                }
                if((choice < StartNo || choice >= LastNo) && choice !=-1)
                    Console.Write("Invalid input, try again:");
            }
            while (choice < StartNo || choice >= LastNo);

            return choice;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Menu menu = new Menu();
        VehicleFleet Mainfleet = new VehicleFleet();
        Mainfleet.ReadData("cars.json");

        do
        {
            menu.DisplayMenu();
            menu.SelectMenu(Mainfleet);
        } while (menu.choice != 0);
    }
}
