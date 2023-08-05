using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPNETWebApp45.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public static List<Customer> GetSampleData()
        {
            var customers = new List<Customer>();

            customers.Add(new Customer
            {
                LastName = "DiCaprio",
                FirstName = "Leonardo",
                BirthDate = new DateTime(1974, 11, 11),
                Phone = "555-1234",
                Email = "leonardo@example.com"
            });

            customers.Add(new Customer
            {
                LastName = "Jolie",
                FirstName = "Angelina",
                BirthDate = new DateTime(1975, 6, 4),
                Phone = "555-5678",
                Email = "angelina@example.com"
            });

            customers.Add(new Customer
            {
                LastName = "Pitt",
                FirstName = "Brad",
                BirthDate = new DateTime(1963, 12, 18),
                Phone = "555-9876",
                Email = "brad@example.com"
            });

            customers.Add(new Customer
            {
                LastName = "Roberts",
                FirstName = "Julia",
                BirthDate = new DateTime(1967, 10, 28),
                Phone = "555-4321",
                Email = "julia@example.com"
            });

            customers.Add(new Customer
            {
                LastName = "Smith",
                FirstName = "Will",
                BirthDate = new DateTime(1968, 9, 25),
                Phone = "555-8765",
                Email = "will@example.com"
            });

            return customers;
        }
    }
}