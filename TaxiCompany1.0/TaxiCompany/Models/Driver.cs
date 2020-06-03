using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaxiCompany.Models
{
    public class Driver
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public int BranchID { get; set; }

        //user IDs from aspNetUser table
        public string OwnerID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name can not be longer than 50 characters.")]
        [Display(Name = "Driver Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "Enter valid phone number")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string Telnumber { get; set; }

        [Required]
        [Display(Name = "Email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public DriverStatus status { get; set; }

        public enum DriverStatus
        {
            Submitted,
            Approved,
            Rejected
        }

        public Branch Branch { get; set; }
    }
}
