using System;

namespace AuroBank_SoftwareProject.Data
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly BankDbContext _bankDbContext;

        private IChargesRepository _chargesRepository;
        private ITransactionRepository _Transaction;
        private IReviewRepository _Review;
        private INotificationRepository _Notification;
        private IBankAccountRepository _bankAccount;
        private IUserRepository _appUser;
        private ILoginRepository _logins;

        public RepositoryWrapper(BankDbContext bankDbContext)
        {
            _bankDbContext = bankDbContext;
        }

        public IBankAccountRepository BankAccount
        {
            get
            {
                if (_bankAccount == null)
                {
                    _bankAccount = new BankAccountRepository(_bankDbContext);
                }

                return _bankAccount;
            }
        }

        public ILoginRepository Logins
        {
            get
            {
                if (_logins == null)
                {
                    _logins = new LoginRepository(_bankDbContext);
                }

                return _logins;
            }
        }

        public ITransactionRepository Transaction
        {
            get
            {
                if (_Transaction == null)
                {
                    _Transaction = new TransactionRepository(_bankDbContext);
                }

                return _Transaction;
            }
        }

        public INotificationRepository Notification
        {
            get
            {
                if (_Notification == null)
                {
                    _Notification = new NotificationRepository(_bankDbContext);
                }

                return _Notification;
            }
        }

        public IChargesRepository Charges
        {
            get
            {
                if (_chargesRepository == null)
                {
                    _chargesRepository = new ChargesRepository(_bankDbContext);
                }

                return _chargesRepository;
            }
        }

        public IReviewRepository Review
        {
            get
            {
                if (_Review == null)
                {
                    _Review = new ReviewRepository(_bankDbContext);
                }

                return _Review;
            }
        }

        public IUserRepository AppUser
        {
            get
            {
                if (_appUser == null)
                {
                    _appUser = new UserRepository(_bankDbContext);
                }

                return _appUser;
            }
        }

        public void SaveChanges()
        {
            _bankDbContext.SaveChanges();
        }
    }
}
