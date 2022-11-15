using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestPages3.ViewModels; // пространство имен моделей RegisterModel и LoginModel
using TestPages3.Models; // пространство имен UserContext и класса User
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TestPages3.Controllers
{
    public class AccountController : Controller
    {
        private AccoutContext db;
        public AccountController(AccoutContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AccessDenied(string action)
        {
            return RedirectToAction("Index","Home");
        }
        
        [HttpGet]
        public IActionResult Login() 
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                Account account = await db.Accounts
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if (account != null)
                {
                    await Authenticate(account); // аутентификация
 
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                Account account = await db.Accounts.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (account == null)
                {
                    // добавляем пользователя в бд
                    account = new Account { Email = model.Email, Password = model.Password,  Name = model.Name};
                    Role userRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                    if (userRole != null)
                    {
                        account.Role = userRole;
                        account.RoleId = 2;
                    }
                    db.Accounts.Add(account);
                    await db.SaveChangesAsync();
 
                    await Authenticate(account); // аутентификация
 
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
 
        private async Task Authenticate(Account user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
 
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}