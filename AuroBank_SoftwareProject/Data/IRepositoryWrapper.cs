namespace AuroBank_SoftwareProject.Data
{
    public interface IRepositoryWrapper
    {
        ITransactionRepository Transaction { get; }
        IReviewRepository Review { get; }
        IChargesRepository Charges { get; }
        IBankAccountRepository BankAccount { get; }
        ILoginRepository Logins { get; }
        INotificationRepository Notification { get; }
        IUserRepository AppUser { get; }

        void SaveChanges();
    }
}
