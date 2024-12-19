using AuroBank_SoftwareProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace AuroBank_SoftwareProject.Data
{
    public class BankDbContext : IdentityDbContext<AppUser>
    {
        public BankDbContext(DbContextOptions<BankDbContext> options) : base(options) 
        {

        }
        
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<LoginSessions> LoginSessions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

    }
}
