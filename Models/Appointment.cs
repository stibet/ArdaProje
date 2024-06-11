using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ArdaProje.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentID { get; set; }

        public int UserID { get; set; }

        public int PTID { get; set; }


        private DateTime _appointmentDateTime;
        public DateTime AppointmentDateTime
        {
            get { return _appointmentDateTime; }
            set
            {
                _appointmentDateTime = value.ToUniversalTime(); // UTC olarak dönüştür; }
            }
        }
        public string Status { get; set; }

        public User User { get; set; }
        public PT PT { get; set; }
    }
}
