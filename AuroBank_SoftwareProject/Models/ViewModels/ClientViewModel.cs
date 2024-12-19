using AuroBank_SoftwareProject.Models;
using System.ComponentModel.DataAnnotations;

namespace AuroBank_SoftwareProject.Controllers
{
    public class ClientViewModel
    {
        public class CombinedUserDataViewModel
        {
            public UserDetailsViewModel UserDetails { get; set; }
            public List<BankAccountViewModel> BankAccounts { get; set; }
            public List<Notification> Notifications { get; set; }
            public List<Transaction> Transactions { get; set; }
        }
        public class ComplainViewModel
        {
            [Required]
            [StringLength(500, ErrorMessage = "Complaint cannot exceed 500 characters.")]
            public string ComplaintText { get; set; }
        }
        public class UserProfileWithBankAccountViewModel
        {
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            [Required]
            public string UserName { get; set; }
            [Required]
            public string IDNumber { get; set; }
            [Required]
            public string MobileNumber { get; set; }
            [Required]
            public string Email { get; set; }

            public string ResidentialAddress { get; set; }
            public string AccountNumber { get; set; }
            public decimal Balance { get; set; } // Add this line

            public List<Reviews> Ratings { get; set; }
        }

        public class UserProfileWithRatingsViewModel
        {
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            [Required]
            public string UserName { get; set; }
            [Required]
            public string IDNumber { get; set; }
            [Required]
            public string MobileNumber { get; set; }
            [Required]
            public string Email { get; set; }

            public string ResidentialAddress { get; set; }
            public string AccountNumber { get; set; }
            public List<Reviews> Ratings { get; set; } // Assuming Reviews is your rating model
        }

        public class UserDetailsViewModel
        {
            
            public string FirstName { get; set; }
            
            public string LastName { get; set; }
            
            public string UserName { get; set; }
            
            public string IDNumber { get; set; }
            
            public string MobileNumber { get; set; }
            
            public string Email { get; set; }

            public string ResidentialAddress { get; set; }
            public string AccountNumber { get; set; }
            public decimal Balance { get; set; }
        }

        public class ViewModel_Ratings
        {
            public List<Reviews> UserReviews { get; set; }
        }
        public class ViewModel_TransferHistory
        {
            public List<Transaction> Transfers { get; set; }
        }

        public class ViewModel_UpdateProfile
        {
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            [Required]
            public string UserName { get; set; }
            [Required]
            public string IDNumber { get; set; }
            [Required]
            public string MobileNumber { get; set; }
            [Required]
            public string Email { get; set; }

            public string ResidentialAddress { get; set; }
            public string AccountNumber { get; set; }
        }

        public class BankAccountViewModel
        {
            public IEnumerable<BankAccount> BankAccount { get; set; }
            public IEnumerable<Transaction> Transactions { get; set; }
        }
        public class ResetPasswordViewModel
        {
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [StringLength(100, ErrorMessage = "Password must be at least {2} characters long.", MinimumLength = 6)]
            public string Password { get; set; }

            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            [Required(ErrorMessage = "Confirm password is required.")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; } // This will hold the reset token
        }
        public class CashSentViewModel
        {
            public int BankAccountId { get; set; }

            [Required]
            public decimal Amount { get; set; }

            public decimal AvailableBalance { get; set; }  // Add this property
        }

        public class BankAccountViewModela
        {
            public string AccountNumber { get; set; }
            public decimal Balance { get; set; }
            public string BankAccountType { get; set; }
        }

        public class MoneyTransferViewModel
        {
            [Required]
            public int SenderBankAccountId { get; set; }

            public int ReceiverBankAccountId { get; set; }

            [Required(ErrorMessage = "The amount is required.")]
            [Range(0.01, double.MaxValue, ErrorMessage = "The amount must be greater than zero.")]
            public decimal Amount { get; set; }

            public string SenderBankAccountNumber { get; set; }

            [Required(ErrorMessage = "The receiver's bank account number is required.")]
            public string ReceiverBankAccountNumber { get; set; }

            public decimal AvailableBalance { get; set; }
        }
    }
}
