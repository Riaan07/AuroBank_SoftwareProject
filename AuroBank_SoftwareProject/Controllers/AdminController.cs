using AuroBank_SoftwareProject.Data;
using AuroBank_SoftwareProject.Models;
using AuroBank_SoftwareProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuroBank_SoftwareProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IRepositoryWrapper _wrapper;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(IRepositoryWrapper wrapper, UserManager<AppUser> userManager)
        {
            _wrapper = wrapper;
            _userManager = userManager;
        }
        public async Task<IActionResult> Users()
        {
            var users = await _wrapper.AppUser.GetAllUsersAndBankAccount();
            var userPageViewModel = new UserPageViewModel()
            {
                AppUsers = users
            };

            return View(userPageViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            await _wrapper.Transaction.RemoveAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AdminUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var results = await _userManager.DeleteAsync(user);
                if (results.Succeeded)
                {
                    return RedirectToAction("Index", "Admin");
                }
                return View();
            }
            return View();
        }

        //[HttpGet]
        //public async Task<IActionResult> GenerateReport()
        //{
        //    try
        //    {
        //        List<string> data = new List<string>();
        //        var reportContent = $"Banking Application\n{DateTime.Now:yyyyMMddHHmmss}\n\n" +
        //            $"***Users\\Clients***\n" +
        //            $"=====================\n" +
        //            $"Account No\tFirst Name\tLast Name\tEmail Address\tStudent Number\n\n";
        //        var report = _userManager.Users;
        //        foreach (var u in report)
        //        {
        //            if (await _userManager.IsInRoleAsync(u, "User"))
        //            {
        //                data.Add($"{u.AccountNumber}\t{u.FirstName}\t{u.LastName}\t{u.Email}\t{u.StudentStaffNumber}\n");
        //            }
        //        }
        //        reportContent += string.Join('\n', data.ToArray());

        //        reportContent += $"\n***All Transactions***\n" +
        //                         $"==========================\n" +
        //            $"Account No\tFirst Name\tLast Name\tEmail Address\tStudent Number\n\n";
        //        var transactions = await _wrapper.Transaction.GetAllAsync();
        //        reportContent += string.Join('\n', transactions.Select(u => $"{u.UserEmail}\t{u.Amount}\t{u.BankAccountIdReceiver}\t{u.BankAccountIdSender}\n").ToArray());

        //        var contentBytes = Encoding.UTF8.GetBytes(reportContent);
        //        var fileName = $"Report_{DateTime.Now:yyyyMMddHHmmss}.txt";

        //        return File(contentBytes, "text/plain", fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error generating report: {ex.Message}");
        //    }
        //}

        //public async Task<IActionResult> systemUser(string email)
        //{
        //    var user = await _userManager.FindByEmailAsync(email);
        //    if (user != null)
        //    {
        //        return View(new ConsultantUpdateUserModel
        //        {
        //            AccountNumber = user.AccountNumber,
        //            Email = user.Email,
        //            LastName = user.LastName,
        //            FirstName = user.FirstName,
        //        });
        //    }
        //    return View();
        //}

        public async Task<IActionResult> Index(string currentPage = "index")
        {
            var viewModel = new IndexPageViewModel
            {
                CurrentPage = currentPage,
                Transactions = await _wrapper.Transaction.GetAllAsync(),
                Advisors = (await _userManager.GetUsersInRoleAsync("Advisor")).ToList(),
                Advices = (await _wrapper.Notification.GetAllAsync()).Where(n => n.IsAdvice).ToList(),
                Consultants = (await _userManager.GetUsersInRoleAsync("Consultant")).ToList(),
                Users = (await _userManager.GetUsersInRoleAsync("User")).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AUsers()
        {
            var users = await _wrapper.AppUser.GetAllUsersAndBankAccount();
            var userPageViewModel = new UserPageViewModel()
            {
                AppUsers = users
            };

            return View(userPageViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Consultants()
        {
            var users = (await _userManager.GetUsersInRoleAsync("Consultant")).ToList();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> ViewAdvices()
        {
            var allAdvices = await _wrapper.Notification.GetAllAsync();
            return View(allAdvices.Where(n => n.Message.StartsWith("[ADVICE]")));
        }

        [HttpGet]
        public async Task<IActionResult> AssignRole(string role, string email)
        {
            if (role != null && email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (await _userManager.IsInRoleAsync(user, "User"))
                    await _userManager.RemoveFromRoleAsync(user, "User");

                string Role = (role == "c") ? "Consultant" : "Advisor";
                var result = await _userManager.AddToRoleAsync(user, Role);
                if (result.Succeeded)
                {
                    Message = $"User successfully assigned to {Role}";
                    return RedirectToAction("Index", "Admin");
                }
            }
            Message = $"Failed to assign user to role";
            return RedirectToAction("Index", "Admin");
        }

        //delete transaction
        [HttpPost]
        public async Task<IActionResult> RemoveTransaction(int id)
        {
            await _wrapper.Transaction.RemoveAsync(id);
            return RedirectToAction("Index");
        }

        [TempData]
        public string Message { get; set; }

        [HttpGet]
        public async Task<IActionResult> ViewAllLogins(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var allLogins = await _wrapper.Logins.GetAllAsync();
                var userBankAccount = (await _wrapper.BankAccount.GetAllAsync()).FirstOrDefault(bc => bc.AccountNumber == user.AccountNumber);
                return View(new ConsultantViewModel
                {
                    SelectedUser = user,
                    loginSessions = allLogins.Where(u => u.UserEmail == email).OrderBy(l => l.TimeStamp)
                });
            }
            return View("Index");
        }

        // GET: Deposit
        public ActionResult Deposit(string email)
        {
            ViewBag.UserEmail = email;
            return View();
        }

        // POST: Deposit
        [HttpPost]
        public async Task<ActionResult> Deposit(string email, decimal amount)
        {
            if (amount <= 0)
            {
                ModelState.AddModelError("", "Amount must be greater than zero.");
                return View();
            }

            var allBankAccounts = await _wrapper.BankAccount.GetAllAsync();


            var bankAccount = allBankAccounts.FirstOrDefault(b => b.UserEmail == email);

            if (bankAccount == null)
            {
                ModelState.AddModelError("", "Bank account not found.");
                return View();
            }

            bankAccount.Balance += amount;
            await _wrapper.BankAccount.UpdateAsync(bankAccount);

            // Save transaction
            var transaction = new Transaction
            {
                BankAccountIdSender = bankAccount.Id,
                Amount = amount,
                TransactionDate = DateTime.UtcNow,
                Reference = "Deposit",
                UserEmail = email
            };
            await _wrapper.Transaction.AddAsync(transaction);

            // Notify user of successful deposit
            var notification = new Notification
            {
                Message = $"You have deposited {amount:C} into your account.",
                NotificationDate = DateTime.UtcNow,
                IsRead = false,
                UserEmail = email
            };
            await _wrapper.Notification.AddAsync(notification);

            return RedirectToAction("Index", "Home");
        }

        // GET: Withdraw
        public ActionResult Withdraw(string email)
        {
            ViewBag.UserEmail = email;
            return View();
        }

        // POST: Withdraw
        [HttpPost]
        public async Task<ActionResult> Withdraw(string email, decimal amount)
        {
            if (amount <= 0)
            {
                ModelState.AddModelError("", "Amount must be greater than zero.");
                return View();
            }

            var allBankAccounts = await _wrapper.BankAccount.GetAllAsync();


            var bankAccount = allBankAccounts.FirstOrDefault(b => b.UserEmail == email);


            if (bankAccount == null)
            {
                ModelState.AddModelError("", "Bank account not found.");
                return View();
            }

            if (bankAccount.Balance < amount)
            {
                ModelState.AddModelError("", "Insufficient balance.");
                return View();
            }

            bankAccount.Balance -= amount;
            await _wrapper.BankAccount.UpdateAsync(bankAccount);

            // Save transaction
            var transaction = new Transaction
            {
                BankAccountIdSender = bankAccount.Id,
                Amount = -amount,
                TransactionDate = DateTime.UtcNow,
                Reference = "Withdrawal",
                UserEmail = email
            };
            await _wrapper.Transaction.AddAsync(transaction);

            // Notify user of successful withdrawal
            var notification = new Notification
            {
                Message = $"You have withdrawn {amount:C} from your account.",
                NotificationDate = DateTime.UtcNow,
                IsRead = false,
                UserEmail = email
            };
            await _wrapper.Notification.AddAsync(notification);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> AdminDeleteUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var results = await _userManager.DeleteAsync(user);
                if (results.Succeeded)
                {
                    return RedirectToAction("Index", "Admin");
                }
                return View();
            }
            return View();
        }

        public async Task<IActionResult> ManageUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return View(new ConsultantUpdateUserModel
                {
                    AccountNumber = user.AccountNumber,
                    Email = user.Email,
                    LastName = user.LastName,
                    FirstName = user.FirstName,
                });
            }
            return View();
        }

        //[HttpGet]
        //public async Task<IActionResult> Report()
        //{
        //    try
        //    {
        //        List<string> data = new List<string>();
        //        var reportContent = $"Banking Application\n{DateTime.Now:yyyyMMddHHmmss}\n\n" +
        //            $"***Users\\Clients***\n" +
        //            $"=====================\n" +
        //            $"Account No\tFirst Name\tLast Name\tEmail Address\tStudent Number\n\n";
        //        var report = _userManager.Users;
        //        foreach (var u in report)
        //        {
        //            if (await _userManager.IsInRoleAsync(u, "User"))
        //            {
        //                data.Add($"{u.AccountNumber}\t{u.FirstName}\t{u.LastName}\t{u.Email}\t{u.StudentStaffNumber}\n");
        //            }
        //        }
        //        reportContent += string.Join('\n', data.ToArray());

        //        reportContent += $"\n***All Transactions***\n" +
        //                         $"==========================\n" +
        //            $"Account No\tFirst Name\tLast Name\tEmail Address\tStudent Number\n\n";
        //        var transactions = await _wrapper.Transaction.GetAllAsync();
        //        reportContent += string.Join('\n', transactions.Select(u => $"{u.UserEmail}\t{u.Amount}\t{u.BankAccountIdReceiver}\t{u.BankAccountIdSender}\n").ToArray());

        //        var contentBytes = Encoding.UTF8.GetBytes(reportContent);
        //        var fileName = $"Report_{DateTime.Now:yyyyMMddHHmmss}.txt";

        //        return File(contentBytes, "text/plain", fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error generating report: {ex.Message}");
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> ManageUser(ConsultantUpdateUserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.LastName = model.LastName;
                    user.FirstName = model.FirstName;

                    var result = await _userManager.UpdateAsync(user);
                    Message = "Updated User Details\n";
                    if (result.Succeeded)
                    {
                        if (model.Password != null && model.ConfirmPassword != null && model.Password == model.ConfirmPassword)
                        {
                            var passResults = await _userManager.RemovePasswordAsync(user);
                            if (passResults.Succeeded)
                            {
                                if ((await _userManager.AddPasswordAsync(user, model.Password)).Succeeded)
                                {
                                    Message += "Successfully updated password";
                                }
                                else
                                {
                                    Message += "Error updating password...Skipping process";
                                }
                            }
                        }
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Could not find user, please contact system admin");
                    Message = "Could not find user, please contact system admin";
                    return View(model);
                }
            }
            return View(model);
        }
    }
}


