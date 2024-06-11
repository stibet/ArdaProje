using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ArdaProje.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        public string FirstName { get; set; }

        //silincek
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

        public int? RemainingLessons { get; set; } // Yeni sütun

        public int PTID { get; set; }

        public PT ?PT { get; set; }

        public ICollection<Appointment> AppointmentRequests { get; set; }
    }
}
