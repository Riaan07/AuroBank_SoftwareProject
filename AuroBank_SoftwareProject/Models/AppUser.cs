using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AuroBank_SoftwareProject.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentStaffNumber { get; set; }
        public string MobileNumber { get; set; }
        public string IDNumber { get; set; }
        public string ResidentialAddress { get; set; }
        public string AccountNumber { get; set; }
    }
}
