using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using caps_in_30.Api.Data;
using caps_in_30.Api.Models;

namespace caps_in_30.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamCardsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeamCardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/teamcards
        [HttpGet]
        public async Task<IActionResult> GetTeamCards(
            [FromQuery] int grade,
            [FromQuery] int? subjectId,
            [FromQuery] int? categoryId)
        {
            var query = _context.TeamCards
                .AsNoTracking()
                .Include(c => c.Subject)
                .Include(c => c.Category)
                .Where(c => c.Grade == grade);

            if (subjectId.HasValue)
            {
                query = query.Where(c => c.SubjectId == subjectId.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == categoryId.Value);
            }

            var cards = await query.ToListAsync();

            var result = cards
                .OrderBy(c => Guid.NewGuid())
                .Select(c => new TeamCardDto
                {
                    Id = c.Id,
                    Grade = c.Grade,
                    Subject = c.Subject.Name,
                    Topic = c.Category.Name,
                    Term = c.Term,
                    ForbiddenWords = System.Text.Json.JsonSerializer
                        .Deserialize<List<string>>(c.ForbiddenWords)
                        ?? new List<string>(),
                    Points = c.Points
                })
                .ToList();

            return Ok(result);
        }

        // GET: api/teamcards/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeamCard(int id)
        {
            var card = await _context.TeamCards
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (card == null)
            {
                return NotFound(new
                {
                    message = "Team card not found."
                });
            }

            return Ok(card);
        }

        // POST: api/teamcards
        [HttpPost]
        public async Task<IActionResult> CreateTeamCard(TeamCard card)
        {
            // Check that the subject exists
            var subjectExists = await _context.Subjects
                .AnyAsync(s => s.Id == card.SubjectId);

            if (!subjectExists)
            {
                return BadRequest(new
                {
                    message = "The specified SubjectId does not exist."
                });
            }

            // Check that the category exists
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == card.CategoryId);

            if (category == null)
            {
                return BadRequest(new
                {
                    message = "The specified CategoryId does not exist."
                });
            }

            // Make sure the category belongs to the subject
            if (category.SubjectId != card.SubjectId)
            {
                return BadRequest(new
                {
                    message = "The specified CategoryId does not belong to the specified SubjectId."
                });
            }

            // Validate the forbidden words JSON
            try
            {
                JsonSerializer.Deserialize<List<string>>(card.ForbiddenWords);
            }
            catch
            {
                return BadRequest(new
                {
                    message = "ForbiddenWords must be a valid JSON array of strings."
                });
            }

            _context.TeamCards.Add(card);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTeamCard),
                new { id = card.Id },
                card
            );
        }

        // DELETE: api/teamcards/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeamCard(int id)
        {
            var card = await _context.TeamCards
                .FirstOrDefaultAsync(c => c.Id == id);

            if (card == null)
            {
                return NotFound(new
                {
                    message = "Team card not found."
                });
            }

            _context.TeamCards.Remove(card);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Team card deleted successfully."
            });
        }
    }
}