using AuroBank_SoftwareProject.Models.ViewModels;
using AuroBank_SoftwareProject.Models;

namespace AuroBank_SoftwareProject.Data
{
    public interface IUserRepository : IRepositoryBase<AppUser>
    {
        Task<List<UserViewModel>> GetAllUsersAndBankAccount();
    }
}
