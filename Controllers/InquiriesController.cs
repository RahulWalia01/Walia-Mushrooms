using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MushroomApi.Data;
using MushroomApi.Models;
using MushroomApi.Services;

namespace MushroomApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InquiriesController : ControllerBase
    {
        private readonly AppDbContext  _db;
        private readonly IEmailService _email;

        public InquiriesController(AppDbContext db, IEmailService email)
        {
            _db    = db;
            _email = email;
        }

        // ── POST /api/inquiries ── Save to DB + Send Email ───────────────────
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Inquiry inquiry)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            inquiry.SubmittedAt = DateTime.UtcNow;
            inquiry.IsRead      = false;

            // 1. Save to database
            _db.Inquiries.Add(inquiry);
            await _db.SaveChangesAsync();

            // 2. Send email notification (don't block response if email fails)
            try
            {
                await _email.SendInquiryNotificationAsync(inquiry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email Error] {ex.Message}");
                // Inquiry is already saved — email failure is non-critical
            }

            return Ok(new { message = "Inquiry received.", id = inquiry.Id });
        }

        // ── GET /api/inquiries ── Fetch all for Admin Panel ──────────────────
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var inquiries = await _db.Inquiries
                .OrderByDescending(i => i.SubmittedAt)
                .ToListAsync();

            return Ok(inquiries);
        }

        // ── GET /api/inquiries/{id} ── Fetch single inquiry ──────────────────
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var inquiry = await _db.Inquiries.FindAsync(id);
            if (inquiry is null) return NotFound();

            return Ok(inquiry);
        }

        // ── PATCH /api/inquiries/{id}/read ── Mark as Read ───────────────────
        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var inquiry = await _db.Inquiries.FindAsync(id);
            if (inquiry is null) return NotFound();

            inquiry.IsRead = true;
            await _db.SaveChangesAsync();

            return Ok(new { message = "Marked as read." });
        }

        // ── DELETE /api/inquiries/{id} ── Delete an inquiry ──────────────────
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var inquiry = await _db.Inquiries.FindAsync(id);
            if (inquiry is null) return NotFound();

            _db.Inquiries.Remove(inquiry);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Deleted." });
        }
    }
}
