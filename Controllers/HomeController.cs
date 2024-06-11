using ArdaProje.Context;
using ArdaProje.Models;
using ArdaProje.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;

namespace ArdaProje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IUserSessionService _userSessionService;

        public HomeController(ApplicationDbContext _db, IUserSessionService userSessionService)
        {
            db = _db;
            _userSessionService = userSessionService;
        }

        public IActionResult Index()
        {
            // Session'dan kullanıcı adını okuyoruz
            string username = _userSessionService.Username;
            int? userId = _userSessionService.UserId;
            int? RemainingLessons = null;
            if (userId > 0) { 
            RemainingLessons = db.Users
                .Where(u => u.UserID == userId)
                .Select(u => u.RemainingLessons)
                .FirstOrDefault();
            }
            ViewBag.RemainingLessons = RemainingLessons;

            // Bu değeri view'a model olarak gönderiyoruz
            return View(model: username);
        }

        [HttpGet]
        public IActionResult GetCalendarEvents()
        {
            var userId = _userSessionService.UserId; // Kullanıcı ID'sini al
            var ptId = _userSessionService.PtID; // PT ID'sini al (null olabilir)

            IQueryable<Appointment> appointmentsQuery;

            if (ptId.HasValue)
            {
                // PT ID mevcutsa, kullanıcı bir PT'dir
                appointmentsQuery = db.Appointments
                    .Where(a => a.PTID == ptId)
                    .Include(a => a.User); // User bilgilerini de çek
            }
            else
            {
                // PT ID yoksa, kullanıcı bir User'dır
                appointmentsQuery = db.Appointments.Where(a => a.UserID == userId);
            }

            var appointments = appointmentsQuery
                .Select(appointment => new
                {
                    id = appointment.AppointmentID,
                    title = ptId.HasValue ? appointment.User.FirstName : "Randevu",
                    start = appointment.AppointmentDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    extendedProps = new
                    {
                        UserID = appointment.UserID,
                        PTID = appointment.PTID,
                        Status = appointment.Status,
                        PTFirstName = appointment.PT.FirstName, // PT'nin FirstName özelliği eklendi
                        IsSelectable = false // Varsayılan olarak seçilebilir
                    }
                }).ToList();

            if (!ptId.HasValue)
            {
                // Kullanıcı bir PT değilse, bağlı olduğu PT'nin ID'sini al
                int? userPtId = db.Users.Where(u => u.UserID == userId).Select(u => u.PTID).FirstOrDefault();

                if (userPtId.HasValue)
                {
                    // Aynı PT'ye bağlı diğer kullanıcıların onaylanmış randevularını eklemek
                    var ptConfirmedAppointments = db.Appointments
                        .Where(a => a.PTID == userPtId && a.Status == "statusOnay" && a.UserID != userId)
                        .Select(a => new
                        {
                            id = a.AppointmentID,
                            title = "Başkasının Randevusu",
                            start = a.AppointmentDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                            extendedProps = new
                            {
                                UserID = a.UserID,
                                PTID = a.PTID,
                                Status = a.Status,
                                PTFirstName = a.PT.FirstName,
                                IsSelectable = true // Kullanıcı bu randevuları seçemesin
                            }
                        }).ToList();

                    // Mevcut randevulara PT'nin onaylı randevularını ekle
                    appointments.AddRange(ptConfirmedAppointments);
                }
            }

            // Tüm kullanıcılar için onaylanmış randevuları eklemek
            var confirmedAppointmentsForAll = db.Appointments
                .Where(a => a.Status == "statusOnay" && (!ptId.HasValue || a.PTID != ptId))
                .Select(a => new
                {
                    id = a.AppointmentID,
                    title = "Onaylanmış Randevu",
                    start = a.AppointmentDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    extendedProps = new
                    {
                        UserID = a.UserID,
                        PTID = a.PTID,
                        Status = a.Status,
                        PTFirstName = a.PT.FirstName,
                        IsSelectable = false // Bu randevular da seçilemez
                    }
                }).ToList();

            // appointments listesindeki tüm randevu ID'lerini toplar
            var existingAppointmentIds = appointments.Select(a => a.id).ToHashSet();

            // confirmedAppointmentsForAll listesinden, zaten appointments listesinde olan randevuları çıkar
            var uniqueConfirmedAppointments = confirmedAppointmentsForAll
                .Where(a => !existingAppointmentIds.Contains(a.id))
                .ToList();

            // Eşsiz onaylanmış randevuları mevcut randevular listesine ekler
            appointments.AddRange(uniqueConfirmedAppointments);

            return Json(appointments);
        }



        [HttpPost]
        public IActionResult DeleteEvent(int id, int userID)
        {
            var ptId = _userSessionService.PtID; // PT ID'sini al (null olabilir)

            if (ptId.HasValue)
            {
                var user = db.Users.SingleOrDefault(u => u.UserID == userID);
                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı." });
                }

                user.RemainingLessons = (user.RemainingLessons ?? 0) + 1; // RemainingLessons değerini artır
            }

            var eventToDelete = db.Appointments.FirstOrDefault(e => e.AppointmentID == id); // Etkinliği ID'ye göre bul
            if (eventToDelete != null)
            {
                db.Appointments.Remove(eventToDelete); // Etkinliği sil
                db.SaveChanges(); // Değişiklikleri kaydet
                return Json(new { success = true, message = "Etkinlik başarıyla silindi." });
            }
            else
            {
                return Json(new { success = false, message = "Etkinlik bulunamadı." });
            }
        }

        [HttpPost]
        public IActionResult UpdateEvent(Appointment model)
        {
            model.UserID = _userSessionService.UserId ?? 1;
            // Modelin null olup olmadığını kontrol edin
            if (model == null)
            {
                return BadRequest("Geçersiz veri");
            }

            // Modelin bir ID'ye sahip olduğundan emin olun (güncelleme için gerekli)
            if (model.AppointmentID == 0)
            {
                return BadRequest("Etkinlik ID'si gereklidir.");
            }

            // Veritabanında ilgili Appointment kaydını bul
            var appointmentToUpdate = db.Appointments.FirstOrDefault(a => a.AppointmentID == model.AppointmentID);

            if (appointmentToUpdate == null)
            {
                return NotFound($"ID'si {model.AppointmentID} olan etkinlik bulunamadı.");
            }

            // Appointment nesnesini güncelle
            appointmentToUpdate.AppointmentDateTime = model.AppointmentDateTime;

            // Değişiklikleri veritabanına kaydet
            db.SaveChanges();

            return Ok("Etkinlik güncellendi");
        }

        [HttpPost]
        public IActionResult AddEvent(Appointment model)
        {
            model.UserID = _userSessionService.UserId ?? model.UserID;
            if (model.UserID != _userSessionService.UserId)
            {
                model.Status = "statusOnay";  // model.UserID ve session UserID eşleşiyorsa, onaylandı olarak güncelle
            }
            else
            {
                model.Status = "statusİstek";  // Eşleşmiyorsa, istek olarak kalır
            }

            var now = DateTime.UtcNow;
            if (model.AppointmentDateTime <= now.AddHours(1))
            {
                // Client'a hata mesajını JSON olarak döndür
                return Json(new { success = false, message = "Etkinlik zamanı geçerli zamandan en az bir saat sonraya ayarlanmalıdır." });
            }

            // UserID'nin "Users" tablosunda mevcut olduğunu kontrol et
            var user = db.Users.FirstOrDefault(user => user.UserID == model.UserID);

            if (user != null)
            {
                model.PT = user.PT;
                model.PTID = user.PTID;
                // User kaydı varsa, Appointment nesnesini ekleyip değişiklikleri kaydet
                db.Appointments.Add(model);
                db.SaveChanges();

                return Json(new { success = true, message = "Etkinlik başarıyla eklenmiştir." });
            }
            else
            {
                // İlgili User kaydı yoksa, bir hata mesajı göster
                ViewBag.ErrorMessage = "İlgili User kaydı bulunamadı.";
                return View(model); // Kullanıcıyı aynı view'a geri yönlendir
            }
        }

        [HttpPost]
        public IActionResult ApproveEvent(int id, int userID)
        {
            var ptId = _userSessionService.PtID; // PT ID'sini al (null olabilir)

            var appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return Json(new { success = false, message = "Etkinlik bulunamadı." });
            }

            if (_userSessionService.PtID != appointment.PTID) // PT kontrolü
            {
                return Json(new { success = false, message = "Etkinlik onaylanamadı veya yetkiniz yok." });
            }

            if (ptId.HasValue)
            {
                var user = db.Users.SingleOrDefault(u => u.UserID == userID);
                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı." });
                }

                user.RemainingLessons = (user.RemainingLessons ?? 0) - 1; // RemainingLessons değerini azalt
            }


            // Randevuyu onayla
            appointment.Status = "statusOnay";

            DateTime utcAppointmentDateTime = appointment.AppointmentDateTime.ToUniversalTime();
            // Aynı zaman dilimindeki diğer randevuları bul ve durumlarını güncelle
            var conflictingAppointments = db.Appointments
                .Where(a => a.AppointmentDateTime == utcAppointmentDateTime && a.AppointmentID != id)
                .ToList();

            foreach (var confApp in conflictingAppointments)
            {
                confApp.Status = "statusRed"; // Çakışan randevuların durumunu "Reddedildi" olarak güncelle
            }

            db.SaveChanges(); // Değişiklikleri veritabanına kaydet

            return Json(new { success = true, message = "Etkinlik onaylandı." });
        }

        [HttpPost]
        public IActionResult RejectEvent(int id, int userID)
        {
            var appointment = db.Appointments.Find(id);

            if (appointment != null && _userSessionService.PtID == appointment.PTID) // PT kontrolü
            {
                appointment.Status = "statusRed";
                db.SaveChanges();
                return Json(new { success = true, message = "Etkinlik reddedildi." });
            }
            return Json(new { success = false, message = "Etkinlik reddedilemedi veya yetkiniz yok." });
        }

        [HttpGet]
        public IActionResult GetClosedSlots()
        {
            var userId = _userSessionService.UserId; // Kullanıcı kimliği alınır
            var ptId = _userSessionService.PtID;
            // Kullanıcının PT'sini bulmak için sorgu
            var user = db.Users
                .Include(u => u.PT) // PT ile ilişki dahil edilerek
                .FirstOrDefault(u =>
                    (userId.HasValue && u.UserID == userId.Value) || // UserID ile kontrol
                    (ptId.HasValue && u.PT.PTID == ptId.Value) // PTID ile kontrol
                );

            if (user?.PT == null)
            {
                return Json(new { success = false, message = "İlişkili PT bulunamadı." }); // Kullanıcı ile ilişkili PT yoksa hata mesajı
            }

            // PT'ye bağlı kapalı slotları alır
            var closedSlots = db.ClosedSlots
                .Where(slot => slot.PTID == user.PT.PTID) // Sadece bu PT'nin kapalı slotları
                .Select(slot => new
                {
                    start = slot.ClosedTime.ToString("yyyy-MM-ddTHH:mm:ssZ"), // Başlangıç zamanı
                    extendedProps = new
                    {
                        PTID = slot.PTID,
                        Status = slot.Status
                    }
                })
                .ToList();

            return Json(closedSlots); // JSON formatında döndür
        }

        [HttpPost]
        public IActionResult CloseTimeSlot(DateTime appointmentDateTime)
        {
            // PT kimliğini alın
            var ptId = _userSessionService.PtID;
            var userId = _userSessionService.UserId;

            if (ptId.HasValue)
            {
                // Yeni kapalı saat oluşturun
                var closedSlot = new ClosedSlots
                {
                    ClosedTime = appointmentDateTime,
                    PTID = ptId.Value,
                    Status = "statusClosed"
                };

                db.ClosedSlots.Add(closedSlot); // Yeni kapalı saat ekleyin
                db.SaveChanges(); // Değişiklikleri kaydedin

                return Json(new { success = true, message = "Saat kapatıldı." });
            }

            return Json(new { success = false, message = "PT kimliği bulunamadı." });
        }

        [HttpPost]
        public IActionResult OpenClosedSlot(DateTime appointmentDateTime)
        {
            // appointmentDateTime'ı UTC'ye çevir
            DateTime utcAppointmentDateTime = appointmentDateTime.ToUniversalTime();

            // utcAppointmentDateTime kullanarak sorgu yap
            var closedSlot = db.ClosedSlots.FirstOrDefault(slot => slot.ClosedTime == utcAppointmentDateTime);

            if (closedSlot == null)
            {
                return Json(new { success = false, message = "Kapalı alan bulunamadı." });
            }

            // Kapalı alanı açmak için gerekli işlemler
            db.ClosedSlots.Remove(closedSlot); // Kapalı alanı silmek için
            db.SaveChanges(); // Değişiklikleri kaydet

            return Json(new { success = true, message = "Kapalı alan açıldı." });
        }

        [HttpGet]
        public async Task<IActionResult> GetPTName(int userId)
        {
            var userIdd = _userSessionService.UserId; // Kullanıcı ID'sini al
            var ptId = _userSessionService.PtID; // PT ID'sini al (null olabilir)

            // Verilen userId ile Users tablosunda arama yap
            var user = await db.Users.FindAsync(userIdd);
            if (user != null)
            {
                // Eğer bulunan bir kullanıcı ise, onun PT'sini bul
                var pt = await db.PTs.FindAsync(user.PTID);
                if (pt != null)
                {
                    return Ok(pt.FirstName); // Kullanıcının PT'sinin adını döndür
                }
                else
                {
                    return NotFound("Bu kullanıcının bir PT'si bulunmamaktadır.");
                }
            }
            else
            {
                // Verilen userId ile PTs tablosunda arama yap
                var ptUser = await db.Users.FirstOrDefaultAsync(u => u.UserID == userId);
                var pt = await db.PTs.FindAsync(ptUser.PTID);
                if (pt != null)
                {
                    // Eğer bulunan bir PT ise, bu PT'nin bir kullanıcısını bul (örneğin, PT'ye atanmış ilk kullanıcıyı bul)
                    //var ptUser = await db.Users.FirstOrDefaultAsync(u => u.PTID == pt.PTID);
                    if (ptUser != null)
                    {
                        return Ok(ptUser.FirstName); // PT'nin bir kullanıcısının adını döndür
                    }
                    else
                    {
                        return NotFound("Bu PT'ye atanmış bir kullanıcı bulunamadı.");
                    }
                }
            }

            return NotFound("Belirtilen ID ile bir kullanıcı veya PT bulunamadı.");
        }

        public async Task<IActionResult> GetUsersByPTId(int ptId)
        {
            // PT ID'sine göre Users tablosundan kullanıcıları sorgula
            var users = await db.Users.Where(u => u.PTID == ptId).ToListAsync();

            if (users != null && users.Count > 0)
            {
                // Kullanıcı listesini, istemciye JSON formatında döndür
                return Json(users.Select(u => new { u.UserID, u.FirstName }));
            }
            else
            {
                // Eğer bu PT'ye atanmış kullanıcı yoksa, hata mesajı döndür
                return NotFound($"PT ID {ptId} ile atanmış kullanıcı bulunamadı.");
            }
        }


    }
}