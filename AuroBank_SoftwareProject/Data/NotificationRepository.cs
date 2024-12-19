using AuroBank_SoftwareProject.Models;
using System;

namespace AuroBank_SoftwareProject.Data
{
    public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
    {
        public NotificationRepository(BankDbContext bankDbContext) : base(bankDbContext)
        {
        }
    }
}
