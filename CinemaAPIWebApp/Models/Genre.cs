using System.ComponentModel.DataAnnotations;

namespace CinemaAPIWebApp.Models
{
    public class Genre
    {
        public Genre()
        {
            MovieGenres = new List<MovieGenre>();
        }
        [Key]
        public int GenreID { get; set; }
        public string Name { get; set; }

        public ICollection<MovieGenre> MovieGenres { get; set; }
    }
}
