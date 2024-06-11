using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ArdaProje.Services;
using ArdaProje.Models;  // Kullanıcı modelinizin bulunduğu namespace
using ArdaProje.Context; // DbContext sınıfınızın bulunduğu namespace
using System.Linq;

public class AccountController : Controller
{
    private readonly ApplicationDbContext db;
    private readonly IUserSessionService _userSessionService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AccountController(ApplicationDbContext context, IUserSessionService userSessionService, IPasswordHasher<User> passwordHasher)
    {
        db = context;
        _userSessionService = userSessionService;
        _passwordHasher = passwordHasher;
    }
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var user = db.Users.FirstOrDefault(u => u.Email == email);
        var pt = db.PTs.FirstOrDefault(p => p.Email == email);

        if (user != null)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            if (result == PasswordVerificationResult.Success)
            {
                _userSessionService.UserId = user.UserID;
                _userSessionService.Username = user.FirstName;
                return RedirectToAction("Index", "Home");
            }
        }

        if (pt != null)
        {
            var ptResult = _passwordHasher.VerifyHashedPassword(user, pt.Password, password);
            if (ptResult == PasswordVerificationResult.Success)
            {
                _userSessionService.PtID = pt.PTID;
                return RedirectToAction("Index", "Home");
            }
        }

        // Kullanıcı bulunamadı veya şifre yanlış
        ViewBag.Error = "Hatalı Giriş Yaptınız.";
        return View();
    }


    public IActionResult LogOut()
    {
        // Oturumdan çıkış işlemleri
        _userSessionService.UserId = null;  // Kullanıcı ID'sini temizle
        _userSessionService.Username = null;  // Kullanıcı adını temizle
        _userSessionService.PtID = null;  // PT ID'sini temizle

        // ASP.NET Core'un yerleşik oturum temizleme yöntemi
        HttpContext.Session.Clear();  // Oturumdaki tüm verileri temizle

        return RedirectToAction("Login");  // Login sayfasına yönlendir
    }
}