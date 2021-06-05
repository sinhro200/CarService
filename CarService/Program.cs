using Core;
using Core.Entities;
using System;

namespace CarService
{
    class Program
    {
        static void Main(string[] args)
        {
            // получение данных
            using (UnitOfWork uow = new UnitOfWork(
                "Host=localhost;Port=5432;Database=carservice;Username=carservice;Password=1234;ENCODING=UTF8"))
            {
                FillDatabase(uow);

                foreach (User u in uow.Users.FindAll()){
                    Console.Out.WriteLine(u.ToString());
                    foreach(Car c in u.Cars)
                    {
                        Console.Out.WriteLine("--Has " + c.ToString());
                    }
                }
            }
        }

        private static void FillDatabase(UnitOfWork unOfWo)
        {
            var users = new User[]
                {
                new User { Id = 1,Name="Tom"},
                new User { Id = 2,Name="Alice"},
                new User { Id = 3,Name="Sam"}
                };
            var brands = new Brand[]
                {
                    new Brand{  Id = 1,Title = "Toyota" },
                    new Brand{Id = 2,Title = "BMW"},
                    new Brand{Id = 3,Title = "Volkswagen"}
                };
            var models = new Model[]
                {
                    new Model{ Id = 1,Title = "Corolla", BrandId = 1},
                    new Model{  Id = 2,Title = "X6", BrandId = 3},
                    new Model{  Id = 3,Title = "Tiguan", BrandId = 2},
                };
            var cars = new Car[]
                {
                    new Car{Id = 1,ModelId = 1, OwnerId = 1 },
                    new Car{Id = 2,ModelId = 2, OwnerId = 3 },
                    new Car{Id = 3,ModelId = 3, OwnerId = 3 },
                };


            unOfWo.Users.SaveAll(users);
            unOfWo.Brands.SaveAll(brands);
            unOfWo.Models.SaveAll(models);
            unOfWo.Cars.SaveAll(cars);
        }
    }
}
