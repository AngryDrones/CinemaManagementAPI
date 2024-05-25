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
    public class CreateTicketRequest
    {
        public int ShowtimeID { get; set; }

        public int SeatNumber { get; set; }
        public int Price { get; set; }
        public DateTime PurchaseTime { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly CinemaAPIContext _context;

        public TicketsController(CinemaAPIContext context)
        {
            _context = context;
        }

        // GET: api/Tickets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetTickets()
        {
            var ticketsInfo = await _context.Tickets
                .Select(t => new
                {
                    t.TicketID,
                    t.ShowtimeID,
                    t.SeatNumber,
                    t.Price,
                    t.PurchaseDate
                })
                .ToListAsync();

            return Ok(ticketsInfo);
        }

        // GET: api/Tickets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return ticket;
        }

        // PUT: api/Tickets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTicket(int id, Ticket ticket)
        {
            if (id != ticket.TicketID)
            {
                return BadRequest();
            }

            _context.Entry(ticket).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(id))
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

        // POST: api/Tickets
        [HttpPost]
        public async Task<ActionResult<Showtime>> PostShowtime(CreateTicketRequest request)
        {
            var showtime = await _context.Showtimes.FindAsync(request.ShowtimeID);

            if (showtime == null)
            {
                return BadRequest(new { message = $"Showtime with ID {request.ShowtimeID} not found." });
            }

            var ticket = new Ticket
            {
                ShowtimeID = request.ShowtimeID,
                SeatNumber = request.SeatNumber,
                Price = request.Price,
                Showtime = showtime
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTicket", new { id = ticket.TicketID }, ticket);
        }

        // DELETE: api/Tickets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.TicketID == id);
        }
    }
}
