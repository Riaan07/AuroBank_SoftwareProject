using AuroBank_SoftwareProject.Models;
using System;

namespace AuroBank_SoftwareProject.Data
{
    public class ReviewRepository : RepositoryBase<Reviews>, IReviewRepository
    {
        public ReviewRepository(BankDbContext bankDbContext) : base(bankDbContext)
        {
        }
    }
}
