using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using caps_in_30.Api.Data;
using caps_in_30.Api.Models;

namespace caps_in_30.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ScoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ScoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/scores
        // Gets all scores
        [HttpGet]
        public async Task<IActionResult> GetScores()
        {
            var scores = await _context.Scores
                .Where(s => s.UserId == GetUserId())
                .OrderByDescending(s => s.Date)
                .ToListAsync();

            return Ok(scores);
        }

        // GET: api/scores/1
        // Gets one score
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScore(int id)
        {
            var userId = GetUserId();

            var score = await _context.Scores
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (score == null)
            {
                return NotFound(new
                {
                    message = "Score not found."
                });
            }

            return Ok(score);
        }

        // POST: api/scores
        // Adds a score for the logged-in user
        [HttpPost]
        public async Task<IActionResult> AddScore(AddScoreDto dto)
        {
            var userId = GetUserId();

            var userExists = await _context.Users
                .AnyAsync(u => u.Id == userId);

            if (!userExists)
            {
                return Unauthorized(new
                {
                    message = "User not found."
                });
            }

            var score = new Score
            {
                UserId = userId,
                Subject = dto.Subject,
                Topic = dto.Topic,
                Grade = dto.Grade,
                Correct = dto.Correct,
                Total = dto.Total,
                Percentage = dto.Percentage,
                TimeUsed = dto.TimeUsed,
                Date = DateTime.UtcNow
            };

            _context.Scores.Add(score);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetScore),
                new { id = score.Id },
                score
            );
        }

        // Gets the ID of the logged-in user from the JWT
        private int GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }

            return int.Parse(userId);
        }
    }
}