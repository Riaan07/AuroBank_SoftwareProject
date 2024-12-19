using AuroBank_SoftwareProject.Data;
using AuroBank_SoftwareProject.Models;
using AuroBank_SoftwareProject.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static AuroBank_SoftwareProject.Controllers.ClientViewModel;


namespace AuroBank_SoftwareProject.Controllers
{
    public class ClientController : Controller
    {
        private readonly IRepositoryWrapper _repo;
        private readonly UserManager<AppUser> _userManager;

        public ClientController(IRepositoryWrapper repo, UserManager<AppUser> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }

        [TempData]
        public string Message { get; set; }

        [HttpGet]
        public IActionResult AddComplain()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddComplain(ComplainViewModel complainModel)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);





                // Create a notification for the user
                var notification = new Notification
                {
                    Message = "Your complaint has been received successfully.",
                    NotificationDate = DateTime.UtcNow,
                    IsRead = false,
                    UserEmail = currentUser.Email
                };

                // Save the notification
                await _repo.Notification.AddAsync(notification);


                // Set a success message and redirect
                Message = "Complaint received successfully!";
                return RedirectToAction("Index", "Client");
            }

            // If model validation fails, return the same view with errors
            return View(complainModel);
        }

        public async Task<IActionResult> UserProfile()
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            var bankAccount = (await _repo.BankAccount.GetAllAsync())
                .FirstOrDefault(b => b.UserEmail == user.Email);

            var model = new UserDetailsViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                AccountNumber = bankAccount?.AccountNumber,
                Balance = bankAccount?.Balance ?? 0
            };

            return View(model);
        }

        public async Task<IActionResult> UserNotifications()
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            var allNotifications = await _repo.Notification.GetAllAsync();
            var userNotifications = allNotifications.Where(n => n.UserEmail == user.Email).ToList();

            return View(userNotifications);
        }

        public async Task<IActionResult> UserTransactions()
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            var allTransactions = await _repo.Transaction.GetAllAsync();
            var userTransactions = allTransactions.Where(t => t.UserEmail == user.Email).ToList();

            return View(userTransactions);
        }

        public async Task<IActionResult> UpdateUserProfile(UpdateProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;
                var user = await _userManager.FindByNameAsync(username);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;

                await _userManager.UpdateAsync(user);

                return RedirectToAction("UserProfile");
            }

            return View(model);
        }

        public async Task<IActionResult> UserRatings()
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            var allRatings = await _repo.Review.GetAllAsync();
            var userRatings = allRatings.Where(r => r.UserEmail == user.Email).ToList();

            return View(userRatings);
        }


        public async Task<IActionResult> IndexAsync()
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            // Retrieve user ratings
            var allRatings = await _repo.Review.GetAllAsync();
            var userRatings = allRatings.Where(r => r.UserEmail == user.Email).ToList();

            // Retrieve bank account information
            var allBankAccounts = await _repo.BankAccount.GetAllAsync();
            var bankAccount = allBankAccounts.FirstOrDefault(b => b.UserEmail == user.Email);

            // Prepare the combined view model
            var model = new UserProfileWithBankAccountViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                AccountNumber = bankAccount?.AccountNumber, // Use null-conditional operator
                Balance = bankAccount?.Balance ?? 0, // Default to 0 if bankAccount is null
                Ratings = userRatings
            };

            return View(model);
        }


        public async Task<IActionResult> Notifications()
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return RedirectToAction("Index", "Client");
            }

            var allNotifications = await _repo.Notification.GetAllAsync();
            var userNotifications = allNotifications.Where(n => n.UserEmail == user.Email).ToList();

            foreach (var notification in userNotifications)
            {
                if (!notification.IsRead)
                {
                    notification.IsRead = true;
                    await _repo.Notification.UpdateAsync(notification);
                    _repo.SaveChanges();
                }
            }

            return View(userNotifications);
        }

        [HttpGet]
        public async Task<IActionResult> Transactions()
        {
            var username = User.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return RedirectToAction("Index", "Client");
            }
            var allTransaction = await _repo.Transaction.GetAllAsync();

            var userTransaction = allTransaction.Where(n => n.UserEmail == user.Email).ToList();

            return View(userTransaction);
        }

        [HttpGet]
        public IActionResult AddRating()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRating(Reviews review)
        {
            var currentLoginUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (User != null)
            {
                review.UserEmail = currentLoginUser.Email;
                review.DateReviewed = DateTime.Now;
                await _repo.Review.AddAsync(review);
                _repo.SaveChanges();
                Message = "Review sent successfully";
                return RedirectToAction("Index", "Client");
            }
            Message = "There was an error sending the review";
            return View(review);
        }


        public async Task<bool> CheckAccounts(string senderEmail, string receiverEmail, decimal amount)
        {
            var allBankAccounts = await _repo.BankAccount.GetAllAsync();


            var senderBankAccount = allBankAccounts.FirstOrDefault(b => b.UserEmail == senderEmail);
            var receiverBankAccount = allBankAccounts.FirstOrDefault(b => b.UserEmail == receiverEmail);


            if (senderBankAccount == null || receiverBankAccount == null)
            {
                return false;
            }

            if (senderBankAccount.Balance < amount)
            {
                return false;
            }


            senderBankAccount.Balance -= amount;
            receiverBankAccount.Balance += amount;


            await _repo.BankAccount.UpdateAsync(senderBankAccount);
            await _repo.BankAccount.UpdateAsync(receiverBankAccount);


            var senderNotification = new Notification
            {
                Message = $"You have sent {amount:C} to account {receiverBankAccount.AccountNumber}.",
                NotificationDate = DateTime.UtcNow,
                IsRead = false,
                UserEmail = senderBankAccount.UserEmail
            };
            var receiverNotification = new Notification
            {
                Message = $"You have received {amount:C} from account {senderBankAccount.AccountNumber}.",
                NotificationDate = DateTime.UtcNow,
                IsRead = false,
                UserEmail = receiverBankAccount.UserEmail
            };

            await _repo.Notification.AddAsync(senderNotification);
            await _repo.Notification.AddAsync(receiverNotification);


            var transaction = new Transaction
            {
                BankAccountIdSender = senderBankAccount.Id,
                BankAccountIdReceiver = receiverBankAccount.Id,
                Amount = amount,
                TransactionDate = DateTime.UtcNow,
                Reference = $"Transfer to {receiverBankAccount.AccountNumber}",
                UserEmail = senderBankAccount.UserEmail
            };

            await _repo.Transaction.AddAsync(transaction);

            return true;
        }

        [HttpGet]
        public async Task<IActionResult> TransferMoneyView()
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            // Fetch all bank accounts
            var allBankAccounts = await _repo.BankAccount.GetAllAsync();

            // Filter user-specific accounts
            var userAccounts = allBankAccounts.Where(b => b.UserEmail == user.Email).ToList();

            var mainBankAccount = userAccounts.FirstOrDefault();

            var viewModel = new MoneyTransferViewModel
            {
                SenderBankAccountNumber = mainBankAccount?.AccountNumber,
                AvailableBalance = mainBankAccount?.Balance ?? 0,
                ReceiverBankAccountNumber = "",

                // Fetch all available accounts for the dropdown

            };

            return View(viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> TransferMoneyView(MoneyTransferViewModel model)
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            var allBankAccounts = await _repo.BankAccount.GetAllAsync();
            var mainBankAccount = allBankAccounts.FirstOrDefault(b => b.UserEmail == user.Email);

            string senderAccountNumber = mainBankAccount.AccountNumber;
            string receiverAccountNumber = model.ReceiverBankAccountNumber;
            decimal amount = model.Amount;

            var senderBankAccount = allBankAccounts.FirstOrDefault(b => b.AccountNumber == senderAccountNumber);
            var receiverBankAccount = allBankAccounts.FirstOrDefault(b => b.AccountNumber == receiverAccountNumber);

            if (senderBankAccount == null || receiverBankAccount == null)
            {
                return RedirectToAction("Index", "Client");
            }

            if (senderBankAccount.Balance < amount)
            {
                return RedirectToAction("Index", "Client");
            }

            senderBankAccount.Balance -= amount;
            receiverBankAccount.Balance += amount;

            await _repo.BankAccount.UpdateAsync(senderBankAccount);
            await _repo.BankAccount.UpdateAsync(receiverBankAccount);

            var senderNotification = new Notification
            {
                Message = $"You have sent {amount:C} to account {receiverBankAccount.AccountNumber}.",
                NotificationDate = DateTime.UtcNow,
                IsRead = false,
                UserEmail = senderBankAccount.UserEmail
            };
            var receiverNotification = new Notification
            {
                Message = $"You have received {amount:C} from account {senderBankAccount.AccountNumber}.",
                NotificationDate = DateTime.UtcNow,
                IsRead = false,
                UserEmail = receiverBankAccount.UserEmail
            };

            await _repo.Notification.AddAsync(senderNotification);
            await _repo.Notification.AddAsync(receiverNotification);
            return RedirectToAction("TransferSuccess", new { amount, receiverAccount = receiverAccountNumber });
        }

        public IActionResult TransferSuccess(decimal amount, string receiverAccount)
        {
            var model = new TransferSuccessViewModel
            {
                Amount = amount,
                ReceiverAccount = receiverAccount
            };
            return View(model);
        }
    }
}

