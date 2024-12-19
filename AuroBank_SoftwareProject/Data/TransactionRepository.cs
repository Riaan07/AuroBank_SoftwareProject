using AuroBank_SoftwareProject.Models;

namespace AuroBank_SoftwareProject.Data
{
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
    {
        public TransactionRepository(BankDbContext bankDbContext) : base(bankDbContext)
        {

        }
    }
}
