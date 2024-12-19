using AuroBank_SoftwareProject.Models;

namespace AuroBank_SoftwareProject.Controllers
{
    public class ConsultantViewModel
    {

        public IQueryable<AppUser> appUsers { get; set; }
        public IQueryable<Reviews> Reviews { get; set; }
        public IEnumerable<LoginSessions> loginSessions { get; set; }
        public AppUser SelectedUser { get; set; }
        public string CurrentPage { get; set; }
    }

    public class ConsultantDepositModel
    {
        public string UserEmail { get; set; }
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
