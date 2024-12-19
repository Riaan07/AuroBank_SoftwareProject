using AuroBank_SoftwareProject.Models;

namespace AuroBank_SoftwareProject.Controllers
{
    public class AdvisorViewModel
    {
        public List<Transaction> Transactions { get; set; }
        public BankAccount CurrentUserBankAccount { get; set; }
        public string UserEmail { get; set; }
        public string Advise { get; set; }
    }
}
