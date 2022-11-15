using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TestPages3.Models;

namespace TestPages3.Controllers
{
    [Authorize(Roles = "admin, user")]
    public class HomeController : Controller
    {
        private ApplicationContext db;
        private readonly ILogger<HomeController> _logger;
        
        public HomeController(ApplicationContext context,ILogger<HomeController> logger, UpTimeServiceSeconds timeService)
        {
            db = context;
            _logger = logger;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await db.Users.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(User user, string action)
        {
            if(action=="Back")
            {
                return RedirectToAction("Index");
            }
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }
        
        [Authorize(Roles = "admin")]
        public IActionResult UpTime([FromServices] UpTimeServiceSeconds timeService)
        {
            return View(timeService);
        }
        
        public async Task<IActionResult> Details(int? id, string action)
        {
            if (id == null) return NotFound();
            
            if(action=="Back")
            {
                return RedirectToAction("Index");
            }
            User user = await db.Users.FirstOrDefaultAsync(p => p.Id == id);
            if (user != null)
                return View(user);
            
            return NotFound();
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            User user;
            if (id == null || (user = await db.Users.FirstOrDefaultAsync(p=>p.Id==id)) == null)
                return NotFound();
            return View(user);
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Id == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, string action)
        {
            if (id == null) return NotFound();
            if (action == "Back")
            {
                return RedirectToAction("Index");
            }
            User user = await db.Users.FirstOrDefaultAsync(p => p.Id == id);
            if (user != null)
            {
                db.Users.Remove(user);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return NotFound();
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(User user, string action)
        {
            if (action == "Back")
            {
                return RedirectToAction("Index");
            }
            db.Users.Update(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
