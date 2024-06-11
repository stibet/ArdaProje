using System.ComponentModel.DataAnnotations;

namespace ArdaProje.Models
{
    public class PT
    {
        [Key]
        public int PTID { get; set; }

        public string FirstName { get; set; }

        //silincek
        public string LastName { get; set; }

        public string ExpertiseArea { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public ICollection<User> Users { get; set; }
        public ICollection<Appointment> AppointmentRequests { get; set; }
        public ICollection<ClosedSlots> ClosedSlots { get; set; }
    }
}
