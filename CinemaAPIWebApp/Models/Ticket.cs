using System.ComponentModel.DataAnnotations;

namespace CinemaAPIWebApp.Models
{
    public class Ticket
    {
        [Key]
        public int TicketID { get; set; }
        public int ShowtimeID { get; set; }
        public Showtime Showtime { get; set; }
        public int SeatNumber { get; set; }
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
