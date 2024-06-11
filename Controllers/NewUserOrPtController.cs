using ArdaProje.Context;
using ArdaProje.Models;
using ArdaProje.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ArdaProje.Controllers
{
    public class NewUserOrPtController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IPasswordHasher<User> _passwordHasher;

        public NewUserOrPtController(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
        {
            db = context;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public async Task<IActionResult> NewUserOrPt()
        {
            // PTs tablosundan tüm PT'leri çek ve FullName oluştur
            var pts = db.PTs.Select(pt => new
            {
                pt.PTID,
                FullName = pt.FirstName + " " + pt.LastName
            }).ToList();

            // SelectList oluştur ve ViewBag üzerinden view'a gönder
            ViewBag.PTSelectList = new SelectList(pts, "PTID", "FullName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewUserOrPt(User model, bool IsPT)
        {
            if (IsPT)
            {
                // PT'nin zaten olup olmadığını kontrol et
                var existingPT = await db.PTs
                                         .FirstOrDefaultAsync(pt => pt.Email == model.Email);
                if (existingPT == null)
                {
                    var pt = new PT
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Password = _passwordHasher.HashPassword(model, model.Password),
                        ExpertiseArea = "PT"
                    };

                    db.PTs.Add(pt); // Eğer PT zaten yoksa, yeni bir PT ekliyoruz.
                    TempData["SuccessMessage"] = "İşlem başarıyla gerçekleştirildi.";
                }
                else
                {
                    TempData["ErrorMessage"] = "PT zaten kayıtlı.";
                }
            }
            else
            {
                // Kullanıcının zaten olup olmadığını kontrol et
                var existingUser = await db.Users
                                           .FirstOrDefaultAsync(user => user.Email == model.Email);
                if (existingUser == null)
                {
                    var user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Password = _passwordHasher.HashPassword(model, model.Password),
                        PTID = model.PTID, // Kullanıcının seçtiği PTID'yi atayın
                        RemainingLessons = model.RemainingLessons // Kullanıcının seçtiği PTID'yi atayın
                    };

                    db.Users.Add(user); // Eğer kullanıcı zaten yoksa, yeni bir kullanıcı ekliyoruz.
                    TempData["SuccessMessage"] = "İşlem başarıyla gerçekleştirildi.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Kullanıcı zaten kayıtlı.";
                }
            }

            await db.SaveChangesAsync();
            
            var pts = db.PTs.Select(pt => new
            {
                pt.PTID,
                FullName = pt.FirstName + " " + pt.LastName
            }).ToList();

            // SelectList oluştur ve ViewBag üzerinden view'a gönder
            ViewBag.PTSelectList = new SelectList(pts, "PTID", "FullName");
            return View(model);
        }
    }
}
