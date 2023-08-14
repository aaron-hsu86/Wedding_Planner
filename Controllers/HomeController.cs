using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _7_Wedding_Planner.Models;
using Microsoft.AspNetCore.Identity;

namespace _7_Wedding_Planner.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    public MyContext db;

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("register")]
    public IActionResult Register(User newUser)
    {
        if (!ModelState.IsValid)
        {
            return View("Index");
        }
        else
        {
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
            db.Users.Add(newUser);
            db.SaveChanges();

            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            HttpContext.Session.SetString("UserName", newUser.FirstName);
            return RedirectToAction("Index", "Wedding");
        }
    }

    [HttpPost("login")]
    public IActionResult Login(LoginUser userCheck)
    {
        if (!ModelState.IsValid)
        {
            return View("Index");
        }
        else
        {
            User? userInDb = db.Users.FirstOrDefault(u => u.Email == userCheck.EmailCheck);
            if (userInDb == null)
            {
                ModelState.AddModelError("EmailCheck", "Invalid Email/Password");
                ModelState.AddModelError("PasswordCheck", "Invalid Email/Password");
                return View("Index");
            }

            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();

            var result = hasher.VerifyHashedPassword(userCheck, userInDb.Password, userCheck.PasswordCheck);
            if(result == 0)
            {
                ModelState.AddModelError("EmailCheck", "Invalid Email/Password");
                ModelState.AddModelError("PasswordCheck", "Invalid Email/Password");
                return View("Index");
            }

            HttpContext.Session.SetInt32("UserId", userInDb.UserId);
            HttpContext.Session.SetString("UserName", userInDb.FirstName);
            return RedirectToAction("Index", "Wedding");
        }
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        // HttpContext.Session.Remove("UserId");
        // HttpContext.Session.Remove("UserName");
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
