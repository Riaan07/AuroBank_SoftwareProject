using AuroBank_SoftwareProject.Models;
using Microsoft.EntityFrameworkCore;
using AuroBank_SoftwareProject.Models.ViewModels;

namespace AuroBank_SoftwareProject.Data
{
    public class UserRepository : RepositoryBase<AppUser>, IUserRepository
    {
        
          private readonly BankDbContext _context;

            public UserRepository(BankDbContext context) : base(context)
            {
                _context = context;
            }

            public async Task<List<UserViewModel>> GetAllUsersAndBankAccount()
            {
                return await (from user in _context.Users
                              join account in _context.BankAccounts
                              on user.Email equals account.UserEmail
                              select new UserViewModel
                              {
                                  AppUser = user,
                                  BankAccount = account,
                              }).ToListAsync();
            }
    }
}
