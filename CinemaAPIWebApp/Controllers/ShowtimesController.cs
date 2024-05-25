using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaAPIWebApp.Models;

namespace CinemaAPIWebApp.Controllers
{
    public class CreateShowtimeRequest
    {
        public int MovieID { get; set; }
        public DateTime ScreeningTime { get; set; }
        public int RoomID { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ShowtimesController : ControllerBase
    {
        private readonly CinemaAPIContext _context;

        public ShowtimesController(CinemaAPIContext context)
        {
            _context = context;
        }

        // GET: api/Showtimes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetShowtimes()
        {
            var showtimesWithRoomAndTickets = await _context.Showtimes
                .Include(s => s.Room)
                .Include(s => s.Tickets)
                .Select(s => new
                {
                    s.ShowtimeID,
                    s.MovieID,
                    s.ScreeningTime,
                    Room = new
                    {
                        s.Room.RoomID,
                        s.Room.Name,
                        s.Room.Capacity
                    },
                    Tickets = s.Tickets.Select(t => new { t.TicketID }).ToList()
                })
                .ToListAsync();

            return Ok(showtimesWithRoomAndTickets);
        }

        // GET: api/Showtimes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Showtime>> GetShowtime(int id)
        {
            var showtime = await _context.Showtimes.FindAsync(id);

            if (showtime == null)
            {
                return NotFound();
            }

            return showtime;
        }

        // PUT: api/Showtimes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShowtime(int id, Showtime showtime)
        {
            if (id != showtime.ShowtimeID)
            {
                return BadRequest();
            }

            _context.Entry(showtime).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShowtimeExists(id))
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

        // POST: api/Showtimes
        [HttpPost]
        public async Task<ActionResult<Showtime>> PostShowtime(CreateShowtimeRequest request)
        {
            var movie = await _context.Movies.FindAsync(request.MovieID);
            var room = await _context.Rooms.FindAsync(request.RoomID);

            if (movie == null)
            {
                return BadRequest(new { message = $"Movie with ID {request.MovieID} not found." });
            }

            if (room == null)
            {
                return BadRequest(new { message = $"Room with ID {request.RoomID} not found." });
            }

            var showtime = new Showtime
            {
                MovieID = request.MovieID,
                ScreeningTime = request.ScreeningTime,
                RoomID = request.RoomID,
                Movie = movie,
                Room = room
            };

            _context.Showtimes.Add(showtime);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetShowtime", new { id = showtime.ShowtimeID }, showtime);
        }

        // DELETE: api/Showtimes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShowtime(int id)
        {
            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime == null)
            {
                return NotFound();
            }

            _context.Showtimes.Remove(showtime);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShowtimeExists(int id)
        {
            return _context.Showtimes.Any(e => e.ShowtimeID == id);
        }
    }
}
