using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaxiCompany.Models
{
    public class Branch
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "City name can not be longer than 50 characters.")]
        [Display(Name = "Branch city")]
        public string City { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Contact Number")]
        public string Contactnumber { get; set; }

        public ICollection<Driver> Drivers { get; set; }
        public ICollection<Customer> Customers { get; set; }
    }
}
