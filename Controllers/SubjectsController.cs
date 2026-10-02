using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using caps_in_30.Api.Data;
using caps_in_30.Api.Models;

namespace caps_in_30.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/subjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subject>>> GetSubjects()
        {
            var subjects = await _context.Subjects
                .AsNoTracking()
                .ToListAsync();

            return Ok(subjects);
        }

        // GET: api/subjects/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Subject>> GetSubject(int id)
        {
            var subject = await _context.Subjects
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null)
            {
                return NotFound(new
                {
                    message = "Subject not found."
                });
            }

            return Ok(subject);
        }

        // POST: api/subjects
        [HttpPost]
        public async Task<ActionResult<Subject>> CreateSubject(Subject subject)
        {
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetSubject),
                new { id = subject.Id },
                subject
            );
        }

        // DELETE: api/subjects/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);

            if (subject == null)
            {
                return NotFound(new
                {
                    message = "Subject not found."
                });
            }

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}