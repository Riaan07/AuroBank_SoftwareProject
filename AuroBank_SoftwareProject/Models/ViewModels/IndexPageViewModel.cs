using AuroBank_SoftwareProject.Models;
namespace AuroBank_SoftwareProject.Controllers

{
   
        public class IndexPageViewModel
        {
            public string CurrentPage { get; set; }
            public List<Transaction> Transactions { get; set; } = new List<Transaction>();
            public List<AppUser> Consultants { get; set; } = new List<AppUser>();
            public List<AppUser> Advisors { get; set; } = new List<AppUser>();
            public List<Notification> Advices { get; set; } = new List<Notification>();
            public List<AppUser> Users { get; set; } = new List<AppUser>();
        }
    
}
