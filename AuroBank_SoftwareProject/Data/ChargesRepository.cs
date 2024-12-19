using AuroBank_SoftwareProject.Models;

namespace AuroBank_SoftwareProject.Data
{
    public class ChargesRepository : RepositoryBase<Charges>, IChargesRepository

    {
        public ChargesRepository(BankDbContext bankDbContext) : base(bankDbContext)
        {
        }
    
    }
}
