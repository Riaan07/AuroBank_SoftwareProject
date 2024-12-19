using AuroBank_SoftwareProject.Models;
using System;

namespace AuroBank_SoftwareProject.Data
{
    public class LoginRepository : RepositoryBase<LoginSessions>, ILoginRepository

    {
        public LoginRepository(BankDbContext bankDbContext) : base(bankDbContext)
        {
        }
    }
}
