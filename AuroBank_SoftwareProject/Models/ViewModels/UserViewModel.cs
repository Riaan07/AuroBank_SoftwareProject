namespace AuroBank_SoftwareProject.Models.ViewModels
{
    public class UserViewModel
    {
        public AppUser AppUser { get; set; }
        public BankAccount BankAccount { get; set; }
        public string _fullName { get; set; } = string.Empty;
    }
}
