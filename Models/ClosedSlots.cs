using System.ComponentModel.DataAnnotations;

namespace ArdaProje.Models
{
    public class ClosedSlots
    {
        public int Id { get; set; } // Birincil anahtar
        private DateTime _closedTime;
        public DateTime ClosedTime
        {
            get { return _closedTime; }
            set
            {
                _closedTime = value.ToUniversalTime(); // UTC olarak dönüştür;
            }
        }
        public string Status { get; set; }
        public int PTID { get; set; } // Kapatan PT'nin ID'si
    }

}
