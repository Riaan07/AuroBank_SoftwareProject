using System;
using AuroBank_SoftwareProject.Models;

namespace AuroBank_SoftwareProject.Data
{
    public class BankAccountRepository : RepositoryBase<BankAccount>, IBankAccountRepository
    {
        private readonly BankDbContext _context;

        public BankAccountRepository(BankDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
