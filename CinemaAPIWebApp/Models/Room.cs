using System.ComponentModel.DataAnnotations;

namespace CinemaAPIWebApp.Models
{
    public class Room
    {
        public Room()
        {
            Showtimes = new List<Showtime>();
        }
        [Key]
        public int RoomID { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }

        public ICollection<Showtime> Showtimes { get; set; }
    }
}
