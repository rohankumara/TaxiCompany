using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaxiCompany.Models
{
    public class Customer
    {
        public int ID { get; set; }
        public int BranchID { get; set; }

        //user IDs from aspNetUser table
        public string OwnerID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Last name can not be longer than 50 characters.")]
        [Display(Name = "Last Name")]
        public string Lastname { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "First name can not be longer than 50 characters.")]
        [Display(Name = "First Name")]
        public string Firstname { get; set; }

        [Display(Name = "Full name")]
        public string Fullname
        {
            get { return Firstname + " " + Lastname; }
        }

        [Required]
        [StringLength(50, ErrorMessage = "position can not be longer than 50 characters.")]
        [Display(Name = "Job Position")]
        public string Career { get; set; }

        [Display(Name = "Gender")]
        public Gender gender { get; set; }

        public int? Age { get; set; }

        [Required]
        [Display(Name = "Home Address")]
        public string Homeaddress { get; set; }

        [Required]
        [Display(Name = "Office Address")]
        public string Officeaddress { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "Please enter valid Telephone number.")]
        [Display(Name = "Tel Number")]
        [DataType(DataType.PhoneNumber)]
        public string Telnumber { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public Branch Branch { get; set; }

        public enum Gender
        {
            Male, Female
        }

        public CustomerStatus Status { get; set; }

        public enum CustomerStatus
        {
            Submitted,
            Approved,
            Rejected
        }
    }
}
