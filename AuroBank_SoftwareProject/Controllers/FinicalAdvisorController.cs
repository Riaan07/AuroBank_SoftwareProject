using AuroBank_SoftwareProject.Data;
using AuroBank_SoftwareProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuroBank_SoftwareProject.Controllers
{
    [Authorize(Roles = "Advisor")]
    public class FinicalAdvisorController : Controller
    {
       
            private readonly UserManager<AppUser> userManager;
            private readonly IRepositoryWrapper wrapper;

            public FinicalAdvisorController(UserManager<AppUser> _userManager, IRepositoryWrapper wrapper)
            {
                userManager = _userManager;
                this.wrapper = wrapper;
            }
            public async Task<IActionResult> UserProfile(string email)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    Message = "User not found.";
                    return RedirectToAction("Index");
                }

                return View(user);
            }

            [HttpGet]
            public async Task<IActionResult> ViewTransactions(string email)
            {
                var transactions = (await wrapper.Transaction.GetAllAsync()).Where(t => t.UserEmail == email).ToList();
                if (transactions == null || !transactions.Any())
                {
                    Message = "No transactions found for this user.";
                    return RedirectToAction("Index");
                }

                return View(transactions);
            }

            [HttpPost]
            public async Task<IActionResult> UpdateUserProfile(AppUser updatedUser)
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByEmailAsync(updatedUser.Email);
                    if (user != null)
                    {
                        user.FirstName = updatedUser.FirstName;
                        user.LastName = updatedUser.LastName;
                        await userManager.UpdateAsync(user);
                        Message = "Profile updated successfully.";
                        return RedirectToAction("UserProfile", new { email = updatedUser.Email });
                    }
                    Message = "User not found.";
                }
                return View(updatedUser);
            }

            public async Task<IActionResult> ViewNotifications()
            {
                var notifications = await wrapper.Notification.GetAllAsync();
                return View(notifications.Where(n => n.UserEmail == User.Identity.Name).ToList());
            }

            [HttpPost]
            public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
            {
                var notification = await wrapper.Notification.GetByIdAsync(notificationId);
                if (notification != null)
                {
                    notification.IsRead = true;
                    await wrapper.Notification.UpdateAsync(notification);
                    Message = "Notification marked as read.";
                }
                return RedirectToAction("ViewNotifications");
            }

            public async Task<IActionResult> DeleteNotification(int notificationId)
            {
                await wrapper.Notification.RemoveAsync(notificationId);
                Message = "Notification deleted.";
                return RedirectToAction("ViewNotifications");
            }

            [TempData]
            public string Message { get; set; }

            public async Task<IActionResult> Index()
            {
                var bankAccounts = await wrapper.BankAccount.GetAllAsync();
                return View(bankAccounts);
            }

            [HttpGet]
            public async Task<IActionResult> Advice(string email)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    Message = "Could not Find User, Please Try Again";
                    return RedirectToAction("Index", "Advisor");
                }

                var allTransactions = (await wrapper.Transaction.GetAllAsync()).Where(t => t.UserEmail == email).ToList();
                var currentUserBankAccount = (await wrapper.BankAccount.GetAllAsync()).Where(ba => ba.UserEmail == email).FirstOrDefault();

                return View(new AdvisorViewModel
                {
                    UserEmail = user.Email,
                    Transactions = allTransactions,
                    CurrentUserBankAccount = currentUserBankAccount,
                });
            }

            [HttpPost]
            public async Task<IActionResult> Advice(AdvisorViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var notify = new Notification
                    {
                        UserEmail = model.UserEmail,
                        Message = model.Advise + $" - From [{User.Identity.Name}]",
                        NotificationDate = DateTime.Now,
                        IsRead = false,
                        IsAdvice = true,
                    };
                    await wrapper.Notification.AddAsync(notify);
                    wrapper.SaveChanges();
                }
                Message = "Successfully sent advice";
                return RedirectToAction("Index", "Advisor");
            }
    }

    
}
