using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace CinemaAPIWebApp.Models
{
    public class Showtime
    {
        [Key]
        public int ShowtimeID { get; set; }
        public int MovieID { get; set; }
        public Movie Movie { get; set; }
        public DateTime ScreeningTime { get; set; }
        public int RoomID { get; set; }
        public Room Room { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
