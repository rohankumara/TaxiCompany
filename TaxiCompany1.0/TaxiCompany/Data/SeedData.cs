using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaxiCompany.Models;
using TaxiCompany.Authorization;

namespace TaxiCompany.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testuserPW)
        {

            
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                var adminID = await EnsureUser(serviceProvider, testuserPW, "admin@Taxi.com");
                await EnsureRole(serviceProvider, adminID, Constants.TaxiOfficeAdministratorsRole);

                SeedDB(context, adminID);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testuserPW, string UserName)
        {
            var usermanager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await usermanager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new ApplicationUser { UserName = UserName };
                await usermanager.CreateAsync(user, testuserPW);
            }
            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {

            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByIdAsync(uid);
            IR = await userManager.AddToRoleAsync(user, role);

            return IR;

        }

        public static void SeedDB(ApplicationDbContext context, string adminID)
        {

            context.Database.EnsureCreated();

            //check the excisiting Branches
            if (context.Branch.Any())
            {
                return;
            }

            List<Branch> branches = new List<Branch>()
            {
                new Branch(){ City = "Colombo-HeadOffice", Contactnumber = "0115682645"},
                new Branch(){ City = "Kandy", Contactnumber = "0815552635"},
                new Branch(){ City = "Kurunegala", Contactnumber = "0378885236"}
            };
            foreach (var b in branches)
            {
                context.Branch.Add(b);
            }
            context.SaveChanges();

            //Look for any drivers
            if (context.Driver.Any())
            {
                return;
            }

            context.Driver.AddRange(
                new Driver()
                {
                    ID = 1111,
                    Name = "Sarath",
                    Telnumber = "0774526586",
                    Email = "Sarath@example",
                    BranchID = branches.Single(b => b.City == "Kandy").ID,
                    status = Driver.DriverStatus.Approved,
                    OwnerID = adminID
                },
                 new Driver()
                 {
                     ID = 1112,
                     Name = "Nihal",
                     Telnumber = "0711529985",
                     Email = "Nihal@example",
                     BranchID = branches.Single(b => b.City == "Kandy").ID,
                     status = Driver.DriverStatus.Approved,
                     OwnerID = adminID
                 },
                  new Driver()
                  {
                      ID = 1113,
                      Name = "John",
                      Telnumber = "0785698874",
                      Email = "John@example",
                      BranchID = branches.Single(b => b.City == "Kandy").ID,
                      status = Driver.DriverStatus.Rejected,
                      OwnerID = adminID
                  },
                   new Driver()
                   {
                       ID = 1114,
                       Name = "Micheal",
                       Telnumber = "0772256955",
                       Email = "Micheal@example",
                       BranchID = branches.Single(b => b.City == "Kandy").ID,
                       status = Driver.DriverStatus.Approved,
                       OwnerID = adminID
                   },
                    new Driver()
                    {
                        ID = 1115,
                        Name = "Ruwan",
                        Telnumber = "0762563215",
                        Email = "Ruwan@example",
                        BranchID = branches.Single(b => b.City == "Kandy").ID,
                        status = Driver.DriverStatus.Rejected,
                        OwnerID = adminID
                    },
                    new Driver()
                    {
                        ID = 1116,
                        Name = "Saman",
                        Telnumber = "0752568211",
                        Email = "Saman@example",
                        BranchID = branches.Single(b => b.City == "Kandy").ID,
                        status = Driver.DriverStatus.Submitted,
                        OwnerID = adminID,
                    }
                );

           
            context.SaveChanges();

            //Look for any customers
            if (context.Customer.Any())
            {
                return;    //Database has been seeded.
            }

            context.Customer.AddRange(

                new Customer()
                {
                    Lastname = "Perera",
                    Firstname = "Sunil",
                    Career = "Teacher",
                    gender = Customer.Gender.Male,
                    Age = 35,
                    Homeaddress = "No.3, Edverd Road, Kollupitiya",
                    Officeaddress = "Ananda college, Collombo",
                    Telnumber = "0712356852",
                    Email = "SunilP@example.com",
                    BranchID = branches.Single(b => b.City == "Kandy").ID,
                    OwnerID = adminID,
                    Status = Customer.CustomerStatus.Approved
                },
                new Customer()
                {
                    Lastname = "Fernando",
                    Firstname = "Shanuka",
                    Career = "Software Engeneer",
                    gender = Customer.Gender.Male,
                    Age = 25,
                    Homeaddress = "No.55, Kandy Road, Kadawatha",
                    Officeaddress = "Vertusa, Collombo",
                    Telnumber = "0772536852",
                    Email = "ShanukaF@example.com",
                    BranchID = branches.Single(b => b.City == "Kandy").ID,
                    OwnerID = adminID,
                    Status = Customer.CustomerStatus.Approved
                },
                new Customer()
                {
                    Lastname = "Avishka",
                    Firstname = "Pasan",
                    Career = "Student",
                    gender = Customer.Gender.Male,
                    Age = 35,
                    Homeaddress = "No.25, Weliweriya,Gampaha",
                    Officeaddress = "Sabaragamuwa University, Belihuloya",
                    Telnumber = "0715863429",
                    Email = "PasanA@example.com",
                    BranchID = branches.Single(b => b.City == "Kandy").ID,
                    OwnerID = adminID,
                    Status = Customer.CustomerStatus.Approved
                },
                new Customer()
                {
                    Lastname = "Tharuka",
                    Firstname = "Kasuni",
                    Career = "Secretary",
                    gender = Customer.Gender.Female,
                    Age = 31,
                    Homeaddress = "No.555,Colombo 3",
                    Officeaddress = "Peters pvt, Collombo",
                    Telnumber = "0785632541",
                    Email = "KasuniT@example.com",
                    BranchID = branches.Single(b => b.City == "Kandy").ID,
                    OwnerID = adminID,
                    Status = Customer.CustomerStatus.Submitted
                },
                new Customer()
                {
                    Lastname = "Perera",
                    Firstname = "Kamal",
                    Career = "Teacher",
                    gender = Customer.Gender.Male,
                    Age = 35,
                    Homeaddress = "No.85, Awissawella road, Kaduwela",
                    Officeaddress = "Royal college, Collombo",
                    Telnumber = "0775239684",
                    Email = "KamalP@example.com",
                    BranchID = branches.Single(b => b.City == "Kandy").ID,
                    OwnerID = adminID,
                    Status = Customer.CustomerStatus.Rejected
                },
                new Customer()
                {
                    Lastname = "Wikramasinghe",
                    Firstname = "Namal",
                    Career = "Teacher",
                    gender = Customer.Gender.Male,
                    Age = 35,
                    Homeaddress = "No.85, Awissawella road, Kosgama",
                    Officeaddress = "Kaduwela college, Kaduwela",
                    Telnumber = "0715239684",
                    Email = "KamalP@example.com",
                    BranchID = branches.Single(b => b.City == "Kandy").ID,
                    OwnerID = adminID,
                    Status = Customer.CustomerStatus.Rejected
                }
           );

            context.SaveChanges();

        }
    }
}