using System.ComponentModel.DataAnnotations;

namespace CinemaAPIWebApp.Models
{
    public class Movie
    {
        [Key]
        public int MovieID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public DateTime ReleaseDate { get; set; }

        public ICollection<MovieGenre> MovieGenres { get; set; }
        public ICollection<Showtime> Showtimes { get; set; }
    }
}
