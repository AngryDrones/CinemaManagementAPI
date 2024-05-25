using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaAPIWebApp.Models;
using Humanizer.Localisation;

namespace CinemaAPIWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly CinemaAPIContext _context;

        public MoviesController(CinemaAPIContext context)
        {
            _context = context;
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetMovies()
        {
            var movies = await _context.Movies
                .Select(m => new
                {
                    m.MovieID,
                    m.Title,
                    m.Description,
                    m.Duration,
                    m.ReleaseDate,
                    GenreIds = m.MovieGenres.Select(mg => mg.GenreID).ToList(),
                    Genres = m.MovieGenres.Select(mg => new { mg.GenreID, mg.Genre.Name }).ToList(),
                    ShowtimeIds = m.Showtimes.Select(s => s.ShowtimeID).ToList()
                })
                .ToListAsync();

            return Ok(movies);
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return movie;
        }

        // PUT: api/Movies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            if (id != movie.MovieID)
            {
                return BadRequest();
            }

            _context.Entry(movie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Movies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie([FromBody] MovieGenreDto movieGenreDto)
        {
            if (await _context.Movies.AnyAsync(g => g.Title == movieGenreDto.Title))
            {
                return Conflict(new { message = "Movie with the such name already exists." });
            }

            var movie = new Movie
            {
                Title = movieGenreDto.Title,
                Description = movieGenreDto.Description,
                Duration = movieGenreDto.Duration,
                ReleaseDate = movieGenreDto.ReleaseDate
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            foreach (var genreId in movieGenreDto.GenreIds)
            {
                var movieGenre = new MovieGenre
                {
                    MovieID = movie.MovieID,
                    GenreID = genreId
                };
                _context.MovieGenres.Add(movieGenre);
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovie", new { id = movie.MovieID }, movie);
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.MovieID == id);
        }
    }

    public class MovieGenreDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<int> GenreIds { get; set; }
    }
}
