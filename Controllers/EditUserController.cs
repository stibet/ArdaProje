using Microsoft.AspNetCore.Mvc;
using ArdaProje.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using ArdaProje.Context;
using ArdaProje.Services;
using Microsoft.EntityFrameworkCore;

public class EditUserController : Controller
{
    private readonly ApplicationDbContext db;
    private readonly IUserSessionService _userSessionService;

    public EditUserController(ApplicationDbContext _db, IUserSessionService userSessionService)
    {
        db = _db;
        _userSessionService = userSessionService;
    }

    [HttpGet]
    [Route("EditUser/EditUser")]
    public IActionResult EditUser()
    {
        return View();
    }

    [HttpGet]
    [Route("EditUser/Index")]
    public IActionResult Index(string searchTerm = null)
    {
        var users = db.Users
            .Where(u => u.FirstName.Contains(searchTerm) || u.LastName.Contains(searchTerm) || u.Email.Contains(searchTerm))
            .ToList();

        var pts = db.PTs
        .Where(pt => pt.FirstName.Contains(searchTerm) || pt.LastName.Contains(searchTerm) || pt.Email.Contains(searchTerm))
        .ToList();

        ViewBag.Users = users;
        ViewBag.PTs = pts;

        return View();
    }
    [HttpGet]
    [Route("EditUser/EditUser/{id?}")]
    public IActionResult EditUser(int id)
    {
        var user = db.Users.Find(id);
        if (user == null)
        {
            return NotFound();
        }

        var pts = db.PTs.Select(pt => new SelectListItem
        {
            Value = pt.PTID.ToString(),
            Text = pt.FirstName + " " + pt.LastName,
            Selected = pt.PTID == user.PTID
        }).ToList();

        ViewBag.PTSelectList = pts;
        return View(user);
    }

    [HttpPost]
    public IActionResult UpdateUser(User user)
    {
        var dbUser = db.Users.FirstOrDefault(u => u.UserID == user.UserID);
        if (dbUser != null)
        {
            dbUser.FirstName = user.FirstName;
            dbUser.LastName = user.LastName;
            dbUser.Email = user.Email;
            dbUser.PTID = user.PTID; // PTID güncellemesi ekleniyor
            dbUser.RemainingLessons = user.RemainingLessons;

            db.SaveChanges();

            TempData["SuccessMessage"] = "Kullanıcı başarıyla güncellendi.";
            return RedirectToAction("EditUser", user);
        }
        else
        {
            TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
            return RedirectToAction("EditUser", user);
        }
    }
    [HttpPost]
    public IActionResult DeleteUser(int id)
    {
        var user = db.Users.Find(id);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
            return RedirectToAction("EditUser", user);
        }

        db.Users.Remove(user);
        db.SaveChanges();
        TempData["SuccessMessage"] = "Kullanıcı başarıyla silindi.";
        return RedirectToAction("EditUser", user);
    }


    [HttpGet]
    public IActionResult Search(string searchTerm)
    {
        var users = db.Users.Where(u => u.FirstName.Contains(searchTerm) || u.LastName.Contains(searchTerm) || u.Email.Contains(searchTerm)).ToList();
        return View("EditUser", users);
    }
}
