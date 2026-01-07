using System;
using System.Linq;

namespace KooliProjekt.Application.Data
{

    public class SeedData
    {
        private readonly ApplicationDbContext _dbContext;

        public SeedData(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public void Generate()
        {
            
            if (_dbContext.Users.Any())
            {
                return;
            }

            var rng = new Random(123);

            
            for (var i = 0; i < 10; i++)
            {
                var user = new User
                {
                    Name = "User " + (i + 1),
                    Email = "user" + (i + 1) + "@example.com",
                    PasswordHash = "hash" + (i + 1),
                    Phone = "555-00" + (i + 1).ToString("D2")
                };
                _dbContext.Users.Add(user);
            }
            _dbContext.SaveChanges();

            
            for (var i = 0; i < 10; i++)
            {
                var type = new CarType
                {
                    Name = "Type " + (i + 1)
                };
                _dbContext.CarTypes.Add(type);
            }
            _dbContext.SaveChanges();

            
            var carTypes = _dbContext.CarTypes.Select(x => x.Id).ToList();
            for (var i = 0; i < 10; i++)
            {
                var car = new Car
                {
                    NumberPlate = "ABC" + (i + 1).ToString("D3"),
                    TypeId = carTypes[rng.Next(carTypes.Count)],
                    Kmrate = 0.25m + (i * 0.01m),
                    TimeRate = 5m + i,
                    IsAvailable = true
                };
                _dbContext.Cars.Add(car);
            }
            _dbContext.SaveChanges();

            
            var userIds = _dbContext.Users.Select(x => x.Id).ToList();
            var carIds = _dbContext.Cars.Select(x => x.Id).ToList();

            for (var i = 0; i < 10; i++)
            {
                var start = DateTime.Today.AddDays(-(i + 1)).AddHours(9);
                var end = start.AddHours(2);

                var booking = new Booking
                {
                    UserId = userIds[rng.Next(userIds.Count)],
                    CarId = carIds[rng.Next(carIds.Count)],
                    StartTime = start,
                    EndTime = end,
                    KmStart = 1000 + (i * 10),
                    KmEnd = 1000 + (i * 10) + 25,
                    Status = "Finished"
                };
                _dbContext.Bookings.Add(booking);
            }
            _dbContext.SaveChanges();

            
            var bookingIds = _dbContext.Bookings.Select(x => x.Id).Take(10).ToList();
            for (var i = 0; i < 10; i++)
            {
                var invoice = new Invoice
                {
                    BookingId = bookingIds[i],
                    InvoiceDate = DateTime.Today.AddDays(-(i + 1)),
                    Total = 0m
                };
                _dbContext.Invoices.Add(invoice);
            }
            _dbContext.SaveChanges();

            
            var invoiceIds = _dbContext.Invoices.Select(x => x.Id).Take(10).ToList();
            for (var i = 0; i < 10; i++)
            {
                var amount = 20m + (i + 1);

                var line = new InvoiceLine
                {
                    InvoiceId = invoiceIds[i],
                    Description = "Service line " + (i + 1),
                    Amount = amount
                };
                _dbContext.InvoiceLines.Add(line);

                
                var invoice = _dbContext.Invoices.Find(invoiceIds[i]);
                invoice.Total += amount;
            }

            _dbContext.SaveChanges();
        }
    }
}
