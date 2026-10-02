using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using caps_in_30.Api.Data;
using caps_in_30.Api.Models;

namespace caps_in_30.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamGamesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeamGamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/teamgames
        // Gets the current team game
        [HttpGet]
        public async Task<IActionResult> GetTeamGame()
        {
            var game = await _context.TeamGames
                .OrderByDescending(g => g.CreatedAt)
                .FirstOrDefaultAsync();

            if (game == null)
            {
                return NotFound(new
                {
                    message = "No team game found."
                });
            }

            return Ok(game);
        }

        // GET: api/teamgames/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeamGame(int id)
        {
            var game = await _context.TeamGames
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return NotFound(new
                {
                    message = "Team game not found."
                });
            }

            return Ok(game);
        }

        // POST: api/teamgames
        // Creates a new team game
        [HttpPost]
        public async Task<IActionResult> CreateTeamGame(TeamGame game)
        {
            game.Id = 0;
            game.CreatedAt = DateTime.UtcNow;
            game.IsFinished = false;

            _context.TeamGames.Add(game);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTeamGame),
                new { id = game.Id },
                game
            );
        }

        // PUT: api/teamgames/1
        // Updates the current game
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeamGame(
            int id,
            TeamGame updatedGame)
        {
            var game = await _context.TeamGames
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return NotFound(new
                {
                    message = "Team game not found."
                });
            }

            game.Grade = updatedGame.Grade;
            game.Subject = updatedGame.Subject;
            game.Topic = updatedGame.Topic;
            game.NumberOfTeams = updatedGame.NumberOfTeams;
            game.NumberOfRounds = updatedGame.NumberOfRounds;
            game.CurrentTeam = updatedGame.CurrentTeam;
            game.CurrentRound = updatedGame.CurrentRound;
            game.IsFinished = updatedGame.IsFinished;

            await _context.SaveChangesAsync();

            return Ok(game);
        }

        // DELETE: api/teamgames/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeamGame(int id)
        {
            var game = await _context.TeamGames
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return NotFound(new
                {
                    message = "Team game not found."
                });
            }

            _context.TeamGames.Remove(game);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Team game deleted successfully."
            });
        }
    }
}