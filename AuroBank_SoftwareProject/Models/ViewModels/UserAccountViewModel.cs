using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AuroBank_SoftwareProject.Controllers

{
    public class LoginViewModel
    {
        [Required]
        [UIHint("email")]
        public string Email { get; set; }

        [Required]
        [UIHint("password")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; } = "/";
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public string RegisterAs { get; set; } = "studentstaff";
        public string EmailAddress { get; set; }

        [DisplayName("Student or Staff number")]
        [RegularExpression(@"^\d{7}$|^\d{10}$", ErrorMessage = "The number must be either 7 or 10 digits long")]
        public string StudentStaffNumber { get; set; }

        [Required(ErrorMessage = "Please enter first name")]
        [DisplayName("First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter last name")]
        [DisplayName("Last name")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter ID number")]
        [DisplayName("ID Number")]
        [StringLength(13, ErrorMessage = "ID number must be 13 digits", MinimumLength = 13)]
        [RegularExpression(@"^(\+?\d{1,3})?\d{13}", ErrorMessage = "Please enter a valid ID number ")]
        public string IDNumber { get; set; }

        [Required(ErrorMessage = "Please enter residential address")]
        [DisplayName("Residential address")]
        public string ResidentialAddress { get; set; }

        [Required(ErrorMessage = "Please enter mobile number")]
        [DisplayName("Mobile number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\+?\d{1,3})?\d{10}$", ErrorMessage = "Please enter a valid mobile number")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm password")]
        [DisplayName("Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords must match")]
        public string ConfirmPassword { get; set; }
    }

    public class UpdateProfileViewModel
    {
        public string Email { get; set; }
        public string AccountNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string IDNumber { get; set; }
        public string ResidentialAddress { get; set; }
    }

    public class ConsultantUpdateUserModel : UpdateProfileViewModel
    {
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }

    public class TransferSuccessViewModel
    {
        public decimal Amount { get; set; }
        public string ReceiverAccount { get; set; }
    }
}