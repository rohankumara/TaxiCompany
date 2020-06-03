using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaxiCompany.Models;

namespace TaxiCompany.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaxiCompany.Models.Customer> Customer { get; set; }

        public DbSet<TaxiCompany.Models.Driver> Driver { get; set; }

        public DbSet<TaxiCompany.Models.Branch> Branch { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<Customer>().ToTable("Customer");
            builder.Entity<Driver>().ToTable("Driver");
            builder.Entity<Branch>().ToTable("Branch");
        }

        

        

       
    }
}
