using AuroBank_SoftwareProject.Data;
using AuroBank_SoftwareProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static AuroBank_SoftwareProject.Controllers.ClientViewModel;

namespace AuroBank_SoftwareProject.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IRepositoryWrapper wrapper;

        //private readonly string _role = "Client";

        public AccountController(UserManager<AppUser> _userManager,
        SignInManager<AppUser> _signInManager, RoleManager<IdentityRole> _roleManager, IRepositoryWrapper _wrapper)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            roleManager = _roleManager;
            wrapper = _wrapper;
        }
     

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register(string registerAs = "student")
        {
            return View(new RegisterViewModel()
            { RegisterAs = registerAs });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            

            if (ModelState.IsValid)
            {
                AppUser user = new()
                {
                    UserName = registerModel.FirstName + registerModel.LastName,
                    Email = registerModel.EmailAddress,
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    StudentStaffNumber = registerModel.StudentStaffNumber,
                    MobileNumber = registerModel.MobileNumber,
                    IDNumber = registerModel.IDNumber,
                    ResidentialAddress = registerModel.ResidentialAddress,
                   

                };

                // Generate unique account number
                Random rndAccount = new();
                string _randomAccount;
                do
                {
                    _randomAccount = rndAccount.Next(99999999, 999999999).ToString();
                }
                while (userManager.Users.Any(u => u.AccountNumber == _randomAccount));

                user.AccountNumber = _randomAccount;

                IdentityResult result = await userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");

                    // Initialize bank account and transaction
                    var bankAccountMain = new BankAccount
                    {
                        AccountNumber = _randomAccount,
                        Balance = 1500m,
                        UserEmail = user.Email,
                    };
                    await wrapper.BankAccount.AddAsync(bankAccountMain);

                    var transaction = new Transaction
                    {
                        TransactionDate = DateTime.Now,
                        BankAccountIdReceiver = int.Parse(_randomAccount),
                        Amount = 1500m,
                        Reference = "Open New Account deposit",
                        UserEmail = user.Email,
                    };
                    await wrapper.Transaction.AddAsync(transaction);

                    var notification = new Notification
                    {
                        IsRead = false,
                        UserEmail = user.Email,
                        NotificationDate = DateTime.Now,
                        Message = "Your AuroBank account was successfully created. Thank you for choosing us!",
                    };
                    await wrapper.Notification.AddAsync(notification);


                    var signin_result = await signInManager.PasswordSignInAsync(user, registerModel.Password, isPersistent: false, lockoutOnFailure: false);
                    if (signin_result.Succeeded)
                    {
                        var newLogin = new LoginSessions
                        {
                            TimeStamp = DateTime.Now,
                            UserEmail = user.Email,
                        };
                        await wrapper.Logins.AddAsync(newLogin);


                        if (await userManager.IsInRoleAsync(user, "Consultant"))
                            return RedirectToAction("Index", "Consultant");
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    foreach (var error in result.Errors.Select(e => e.Description))
                        ModelState.AddModelError("", error);
                }
            }
            return View(registerModel);
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await signInManager.PasswordSignInAsync(user, model.Password, isPersistent: model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        var newLogin = new LoginSessions
                        {
                            TimeStamp = DateTime.Now,
                            UserEmail = user.Email,
                        };
                        await wrapper.Logins.AddAsync(newLogin);


                        if (await userManager.IsInRoleAsync(user, "Consultant"))
                            return RedirectToAction("Index", "Consultant");
                        return Redirect(model?.ReturnUrl ?? "/Home/Index");
                    }
                }
            }
            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        // New Method: Reset Password
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            return View(new ResetPasswordViewModel { Email = email });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // Set the new password
                    var result = await userManager.ResetPasswordAsync(user, model.Code, model.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User not found.");
                }
            }
            return View(model);
        }


        // New Method: Manage User Roles
        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await userManager.GetRolesAsync(user);
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    await signInManager.SignOutAsync();
                    return RedirectToAction("Login", "Account");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View("AccessDenied");
        }
    }

}

