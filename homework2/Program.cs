using homework_2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace homework_2
{

    // vehicle class
    public abstract class Vehicle
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int YearOfManufacture {  get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public string RegistrationNumber { get; set; }
        public abstract decimal CalculateRentalCost(double duration, double distance);
        public abstract bool NeedsMaintenance(double distance);
        public double DistanceTraveled { get; protected set; }
    }

    // Pessenger vehicle class  
    public class PassengerVehicle : Vehicle
    {
        public double SpecificCarModelCoefficient { get; set; }
        public int LesseeRating { get; set; }

        // calculates rental cost of passenger vehicles
        public override decimal CalculateRentalCost(double duration, double distance)
        {
            return (decimal)(duration * distance * SpecificCarModelCoefficient * LesseeRating);
        }

        // maintenance threshold for passenger vehicles
        public override bool NeedsMaintenance(double distance)
        {
            const int maintenancethreshold = 5000;
            return distance % maintenancethreshold <= 1000;
        }
    }

    // Cargo vehicle class
    public class CargoVehicle : Vehicle
    {
        public double ModelCoefficient { get; set; }
        public double CargoWeight { get; set; }

        // calculates rental cost of cargo vehicles
        public override decimal CalculateRentalCost(double duration, double distance)
        {
            return (decimal)(duration * distance * ModelCoefficient * CargoWeight);
        }

        // maintenance threshold for cargo vehicles
        public override bool NeedsMaintenance(double distance)
        {
            const int maintenancethreshold = 15000;
            return distance % maintenancethreshold <= 1000;
        }
    }   

    // container class
    public class VehicleFleet
    {
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
            var currentdate = DateTime.Now;
            return vehicles.Where(v =>
            {
                if(v.Model.Equals(model, StringComparison.OrdinalIgnoreCase))
                {
                    if(v is PassengerVehicle passengervehicle)
                    {
                        return currentdate.Year - passengervehicle.YearOfManufacture >= 5 ||
                           passengervehicle.NeedsMaintenance(100000);
                    }
                    else if(v is CargoVehicle cargovehicle)
                    {
                        return currentdate.Year - cargovehicle.YearOfManufacture >= 5 ||
                           cargovehicle.NeedsMaintenance(100000);
                    }
                }
                return false;

            }).ToList();
        }

        public decimal CalculateTotalValueOfFleet()
        {
            var currentDate = DateTime.Now;
            return vehicles.Sum(v =>
            {
                if (v is PassengerVehicle passengerVehicle)
                {
                    return passengerVehicle.Price * (decimal)Math.Pow(0.9, currentDate.Year - passengerVehicle.YearOfManufacture);
                }
                else if (v is CargoVehicle cargoVehicle)
                {
                    return cargoVehicle.Price * (decimal)Math.Pow(0.93, currentDate.Year - cargoVehicle.YearOfManufacture);
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
                    return passengerVehicle.NeedsMaintenance(passengerVehicle.DistanceTraveled);
                }
                else if (v is CargoVehicle cargoVehicle)
                {
                    return cargoVehicle.NeedsMaintenance(cargoVehicle.DistanceTraveled);
                }
                return false;
            }).ToList();
        }
    }
}

    class Program
    {
        static void Main(string[] args)
        {
            VehicleFleet fleet = new VehicleFleet();

        // Adding vehicles to the fleet
        fleet.AddVehicle(new PassengerVehicle
        {
            Id = 1,
            Brand = "Toyota",
            Model = "Aygo",
            YearOfManufacture = 2023,
            Color = "Red",
            Price = 19000,
            RegistrationNumber = "DW88888",
            SpecificCarModelCoefficient = 0.06,
            LesseeRating = 2
        });
        fleet.AddVehicle(new PassengerVehicle
            {
                Id = 3,
                Brand = "Toyota",
                Model = "Corolla",
                YearOfManufacture = 2019,
                Color = "Blue",
                Price = 23000,
                RegistrationNumber = "ABC123",
                SpecificCarModelCoefficient = 0.04,
                LesseeRating = 3
            });


            fleet.AddVehicle(new CargoVehicle
            {
                Id = 2,
                Brand = "Volkswagen",
                Model = "Transit",
                YearOfManufacture = 2011,
                Color = "Gray",
                Price = 30000,
                RegistrationNumber = "ABC456",
                ModelCoefficient = 0.05,
                CargoWeight = 1300
            });

        // Querying the fleet
        var vehiclesByBrand = fleet.ListVehiclesByBrand("Toyota");
        Console.WriteLine("a) Count of Toyota's: "+vehiclesByBrand.Count);
        for(int i=0; i<vehiclesByBrand.Count; i++)
        {
            Console.Write("Id:"+vehiclesByBrand[i].Id+"  Model: " + vehiclesByBrand[i].Model+"\n");
        }

        var vehiclesExceedingTenure = fleet.ListVehiclesExceedingTenure("Transit");
        Console.WriteLine("\nb) Count of vehicles with exceeding tenure: " + vehiclesExceedingTenure.Count);
        for (int i = 0; i < vehiclesExceedingTenure.Count; i++)
        {
            Console.Write("Id:" + vehiclesExceedingTenure[i].Id + "  Model: " + vehiclesExceedingTenure[i].Model + "\n");
        }

        var totalFleetValue = fleet.CalculateTotalValueOfFleet();
        Console.WriteLine("\nc) Total fleet value equals: " + totalFleetValue.ToString("0.00"));
        
        var vehiclesByPreference = fleet.ListVehiclesByPreference("Volkswagen", "Gray");
        Console.WriteLine("\nd) Count of vehicles with preferences: " + vehiclesByPreference.Count);
        for (int i = 0; i < vehiclesByPreference.Count; i++)
        {
            Console.Write("Id:" + vehiclesByPreference[i].Id + "  Model: " + vehiclesByPreference[i].Model + "\n");
        }

        var vehiclesRequiringMaintenance = fleet.ListVehiclesRequiringMaintenance();
        Console.WriteLine("\ne) Count of vehicles needing maintenance: " + vehiclesRequiringMaintenance.Count);
        for (int i = 0; i < vehiclesRequiringMaintenance.Count; i++)
        {
            Console.Write("Id:" + vehiclesRequiringMaintenance[i].Id + "  Model: " + vehiclesRequiringMaintenance[i].Model + "\n");
        }

    }
    }
