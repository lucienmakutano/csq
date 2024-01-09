using System;
using System.Collections.Generic;
using System.IO;

namespace CarRental
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.Out.WriteLine("Hello world");
        }
    }

    class WestminsterRentalVehicle: IRentalManager, IRentalCustomer
    {
        private HashSet<Vehicle> _vehicles;
        private int _capacity;

        WestminsterRentalVehicle(HashSet<Vehicle> vehicles, int maximumLots)
        {
            _vehicles = vehicles;
            _capacity = maximumLots;
        }
        
        public bool AddVehicle(Vehicle vehicle)
        {
            if (_capacity <= 0)
            {
                return false;
            }
            
            _vehicles.Add(vehicle);
            _capacity -= 1;
            Console.Out.WriteLine("Available parking lots {0}", _capacity);

            return true;
        }

        public bool DeleteVehicle(String number)
        {
            return _vehicles.RemoveWhere((vehicle) => vehicle.RegistrationNumber == number) == 1;
        }

        public void ListVehicles()
        {
            foreach (var vehicle in _vehicles)
            {
                Console.Out.WriteLine("Registration number: {0}\n" +
                                      "Vehicle type: {1}\n" +
                                      "Reservations {2}", vehicle.RegistrationNumber, vehicle.Make, vehicle.Reservations);
            }
        }

        public void ListOrderedVehicles()
        {
            throw new NotImplementedException();
        }

        public void GenerateReport(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                foreach (var vehicle in _vehicles)
                {
                    writer.WriteLine(vehicle.RegistrationNumber);
                    writer.WriteLine(vehicle.Make);
                    writer.WriteLine(vehicle.Model);
                    writer.WriteLine(vehicle.RentalPrice);
                    writer.WriteLine(vehicle.Reservations);
                    writer.WriteLine();
                }
            }
        }

        public void ListAvailableVehicles(Schedule wantedSchedule, Type type)
        {
            foreach (var vehicle in _vehicles)
            {
                vehicle.Reservations.ForEach((s) =>
                {
                    if (DateTime.Compare(wantedSchedule.PickupDate, s.PickupDate) == 1
                        && DateTime.Compare(s.DropOffDate, wantedSchedule.DropOffDate) == 1)
                    {}
                    else
                    {
                        Console.Out.WriteLine(vehicle);
                    }
                });
            }
        }

        public bool AddReservation(string number, Schedule wantedSchedule)
        {
            foreach (var vehicle in _vehicles)
            {
                if (vehicle.RegistrationNumber == number)
                {
                    foreach (var vehicleReservation in vehicle.Reservations)
                    {
                        if ((DateTime.Compare(wantedSchedule.PickupDate, vehicleReservation.PickupDate) == 1
                             && DateTime.Compare(vehicleReservation.DropOffDate, wantedSchedule.DropOffDate) == 1) ||
                            (DateTime.Compare(vehicleReservation.PickupDate, wantedSchedule.PickupDate) == 1 
                             && DateTime.Compare(vehicleReservation.DropOffDate, wantedSchedule.DropOffDate) == 1))
                        {
                            return false;
                        }

                        vehicle.Reservations.Add(wantedSchedule);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool ChangeReservation(string number, Schedule oldSchedule, Schedule newSchedule)
        {
            throw new NotImplementedException();
        }

        public bool DeleteReservation(string number, Schedule schedule)
        {
            foreach (var vehicle in _vehicles)
            {
                if (vehicle.RegistrationNumber == number)
                {
                    foreach (var vehicleReservation in vehicle.Reservations)
                    {
                        if (vehicleReservation == schedule)
                        {
                            vehicle.Reservations.Remove(schedule);
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }

    abstract class Vehicle: IOverlappable, IComparable
    {
        public string RegistrationNumber { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int RentalPrice { get; set; }
        public List<Schedule> Reservations { get; set; }
        
        public bool Overlaps(Schedule other)
        {
            bool overlap = false;
            
            Reservations.ForEach(schedule =>
            {
                if (DateTime.Compare(schedule.DropOffDate, other.PickupDate) == 1)
                {
                    overlap = true;
                }
            });

            return overlap;
        }

        public int CompareTo(object obj)
        {
            var v = (Vehicle)obj;
            return String.Compare(Make, v.Make);
        }
    }

    class Van : Vehicle {}

    class Car : Vehicle {}

    class ElectricCar: Vehicle {}

    class MotorBike : Vehicle {}

    class RentalBranch { }

    class Driver
    {
        private String Name { get; set; }
        private String Surname { get; set; }
        private DateTime DateOfBirth { get; set; }
    }
    
    class Schedule
    {
        public DateTime PickupDate { get; set; }
        public DateTime DropOffDate { get; set; }
    }

    interface IRentalManager
    {
        bool AddVehicle(Vehicle vehicle);
        bool DeleteVehicle(string number);
        void ListVehicles();
        void ListOrderedVehicles();
        void GenerateReport(String filename);
    }

    interface IRentalCustomer
    {
        void ListAvailableVehicles(Schedule wantedSchedule, Type type);
        bool AddReservation(String number, Schedule wantedSchedule);
        bool ChangeReservation(String number, Schedule oldSchedule, Schedule newSchedule);
        bool DeleteReservation(String number, Schedule schedule);
    }

    interface IOverlappable
    {
        bool Overlaps(Schedule schedule);
    }
}