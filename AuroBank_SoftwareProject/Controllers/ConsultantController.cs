using AuroBank_SoftwareProject.Data;
using AuroBank_SoftwareProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AuroBank_SoftwareProject.Controllers
{
    
        [Authorize(Roles = "Consultant")]
        public class ConsultantController : Controller
        {
            private readonly UserManager<AppUser> userManager;
            private readonly IRepositoryWrapper wrapper;

            public ConsultantController(UserManager<AppUser> _userManager, IRepositoryWrapper _wrapper)
            {
                userManager = _userManager;
                this.wrapper = _wrapper;
            }

            [TempData]
            public string Message { get; set; }

            public async Task<IActionResult> Index(string currentPage = "index")
            {
                List<AppUser> lstUsers = new List<AppUser>();
                foreach (var user in userManager.Users)
                {
                    if (await userManager.IsInRoleAsync(user, "User"))
                        lstUsers.Add(user);
                }
                return View(new ConsultantViewModel
                {
                    appUsers = lstUsers.AsQueryable(),
                    CurrentPage = currentPage,
                    Reviews = (await wrapper.Review.GetAllAsync()).AsQueryable()
                });
            }

            public async Task<IActionResult> ViewAllLogins(string email)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    var allLogins = await wrapper.Logins.GetAllAsync();
                    var userBankAccount = (await wrapper.BankAccount.GetAllAsync()).FirstOrDefault(bc => bc.AccountNumber == user.AccountNumber);
                    return View(new ConsultantViewModel
                    {
                        SelectedUser = user,
                        Reviews = (await wrapper.Review.GetAllAsync()).AsQueryable(),
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

                var allBankAccounts = await wrapper.BankAccount.GetAllAsync();


                var bankAccount = allBankAccounts.FirstOrDefault(b => b.UserEmail == email);

                if (bankAccount == null)
                {
                    ModelState.AddModelError("", "Bank account not found.");
                    return View();
                }

                bankAccount.Balance += amount;
                await wrapper.BankAccount.UpdateAsync(bankAccount);

                // Save transaction
                var transaction = new Transaction
                {
                    BankAccountIdSender = bankAccount.Id,
                    Amount = amount,
                    TransactionDate = DateTime.UtcNow,
                    Reference = "Deposit",
                    UserEmail = email
                };
                await wrapper.Transaction.AddAsync(transaction);

                // Notify user of successful deposit
                var notification = new Notification
                {
                    Message = $"You have deposited {amount:C} into your account.",
                    NotificationDate = DateTime.UtcNow,
                    IsRead = false,
                    UserEmail = email
                };
                await wrapper.Notification.AddAsync(notification);

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

                var allBankAccounts = await wrapper.BankAccount.GetAllAsync();


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
                await wrapper.BankAccount.UpdateAsync(bankAccount);

                // Save transaction
                var transaction = new Transaction
                {
                    BankAccountIdSender = bankAccount.Id,
                    Amount = -amount,
                    TransactionDate = DateTime.UtcNow,
                    Reference = "Withdrawal",
                    UserEmail = email
                };
                await wrapper.Transaction.AddAsync(transaction);

                // Notify user of successful withdrawal
                var notification = new Notification
                {
                    Message = $"You have withdrawn {amount:C} from your account.",
                    NotificationDate = DateTime.UtcNow,
                    IsRead = false,
                    UserEmail = email
                };
                await wrapper.Notification.AddAsync(notification);

                return RedirectToAction("Index", "Home");
            }

            public async Task<IActionResult> DepositWithdraw(string email)
            {
                var user = await userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    return View(new ConsultantDepositModel
                    {
                        AccountNumber = user.AccountNumber,
                        UserEmail = user.Email,
                    });
                }
                return RedirectToAction("Index", "Consultant");
            }

            [HttpPost]
            public async Task<IActionResult> DepositWithdraw(ConsultantDepositModel model, string action)
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByEmailAsync(model.UserEmail);
                    if (user == null)
                    {
                        Message = "Couldn't find user, please contact the system administrator";
                        ModelState.AddModelError("", "Couldn't find user");
                        return View(model);
                    }

                    var AllBankAcc = await wrapper.BankAccount.GetAllAsync();
                    var userBankAcc = AllBankAcc.FirstOrDefault(bc => bc.UserEmail == user.Email);
                    if (userBankAcc == null)
                    {
                        Message = "Couldn't find bank account, please contact the system administrator";
                        ModelState.AddModelError("", "Couldn't find bank account");
                        return View(model);
                    }

                    if (action.ToLower() == "deposit")
                    {
                        userBankAcc.Balance += model.Amount;
                    }
                    else
                    {
                        if (userBankAcc.Balance - model.Amount < -15)
                        {
                            ModelState.AddModelError("", "User has insufficient balance in their account");
                            return View(model);
                        }
                        userBankAcc.Balance -= model.Amount;
                    }

                    await wrapper.BankAccount.UpdateAsync(userBankAcc);

                    await wrapper.Transaction.AddAsync(new Transaction
                    {
                        Amount = model.Amount,
                        UserEmail = model.UserEmail,
                        Reference = $"Deposit/Withdraw cash in account",
                        BankAccountIdReceiver = int.Parse(user.AccountNumber),
                        BankAccountIdSender = 0,
                        TransactionDate = DateTime.Now
                    });
                    wrapper.SaveChanges();
                    await wrapper.Notification.AddAsync(new Notification
                    {
                        IsAdvice = false,
                        IsRead = false,
                        Message = "Money was moved in account",
                        NotificationDate = DateTime.Now,
                        UserEmail = model.UserEmail
                    });
                    wrapper.SaveChanges();

                    Message = $"Money successfully moved from/to account";
                    return RedirectToAction("Index", "Consultant");
                }

                return View(model);
            }

            public async Task<IActionResult> DeleteUser(string email)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    var results = await userManager.DeleteAsync(user);
                    if (results.Succeeded)
                    {
                        return RedirectToAction("Index", "Consultant");
                    }
                    return View();
                }
                return View();
            }

            public async Task<IActionResult> ManageUser(string email)
            {
                var user = await userManager.FindByEmailAsync(email);
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

            [HttpPost]
            public async Task<IActionResult> ManageUser(ConsultantUpdateUserModel model)
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        user.Email = model.Email;
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        var result = await userManager.UpdateAsync(user);
                        Message = "Updated User Details\n";
                        if (result.Succeeded)
                        {
                            if (model.Password != null && model.ConfirmPassword != null && model.Password == model.ConfirmPassword)
                            {
                                var passResults = await userManager.RemovePasswordAsync(user);
                                if (passResults.Succeeded)
                                {
                                    if ((await userManager.AddPasswordAsync(user, model.Password)).Succeeded)
                                    {
                                        Message += "Successfully updated password";
                                    }
                                    else
                                    {
                                        Message += "Error updating password...Skipping process";
                                    }
                                }
                            }
                            return RedirectToAction("Index", "Consultant");
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

            [HttpGet]
            public async Task<IActionResult> GenerateReport()
            {
                try
                {
                    List<string> data = new List<string>();
                    var reportContent = $"Banking Application\n{DateTime.Now:yyyyMMddHHmmss}\n\n" +
                        $"***Users\\Clients***\n" +
                        $"=====================\n" +
                        $"Account No\tFirst Name\tLast Name\tEmail Address\tStudent Number\n\n";
                    var report = userManager.Users;
                    foreach (var u in report)
                    {
                        if (await userManager.IsInRoleAsync(u, "User"))
                        {
                            data.Add($"{u.AccountNumber}\t{u.FirstName}\t{u.LastName}\t{u.Email}\t{u.StudentStaffNumber}\n");
                        }
                    }
                    reportContent += string.Join('\n', data.ToArray());

                    reportContent += $"\n***All Transactions***\n" +
                                     $"==========================\n" +
                        $"Account No\tFirst Name\tLast Name\tEmail Address\tStudent Number\n\n";
                    var transactions = await wrapper.Transaction.GetAllAsync();
                    reportContent += string.Join('\n', transactions.Select(u => $"{u.UserEmail}\t{u.Amount}\t{u.BankAccountIdReceiver}\t{u.BankAccountIdSender}\n").ToArray());

                    var contentBytes = Encoding.UTF8.GetBytes(reportContent);
                    var fileName = $"Report_{DateTime.Now:yyyyMMddHHmmss}.txt";

                    return File(contentBytes, "text/plain", fileName);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error generating report: {ex.Message}");
                }
            }
        }

    
}
