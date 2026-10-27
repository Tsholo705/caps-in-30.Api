
using caps_in_30.Api.Data;
using caps_in_30.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace caps_in_30.Api.Controllers;

[ApiController]
[Route("api/timetables")]
public class TimetablesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TimetablesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET /api/timetables?userId=1
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Timetable>>> GetTimetables(
        [FromQuery] int userId)
    {
        var timetables = await _context.Timetables
            .Include(t => t.Sessions)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedDate)
            .ToListAsync();

        return Ok(timetables);
    }

    // GET /api/timetables/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Timetable>> GetTimetable(int id)
    {
        var timetable = await _context.Timetables
            .Include(t => t.Sessions)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (timetable == null)
        {
            return NotFound(new { message = "Timetable not found." });
        }

        return Ok(timetable);
    }

    // POST /api/timetables
    [HttpPost]
    public async Task<ActionResult<Timetable>> CreateTimetable(
        [FromBody] SaveTimetableRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new
            {
                message = "Please give your timetable a name."
            });
        }

        var userExists = await _context.Users
            .AnyAsync(u => u.Id == request.UserId);

        if (!userExists)
        {
            return NotFound(new { message = "User not found." });
        }

        var timetable = new Timetable
        {
            UserId = request.UserId,
            Name = request.Name.Trim(),
            Description = (request.Description ?? "").Trim(),
            CreatedDate = DateTime.UtcNow,
            Sessions = (request.Sessions ?? new List<StudySessionInput>())
                .Select(ToSession)
                .ToList()
        };

        _context.Timetables.Add(timetable);
        await _context.SaveChangesAsync();

        return Ok(timetable);
    }

    // PUT /api/timetables/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Timetable>> UpdateTimetable(
        int id,
        [FromBody] UpdateTimetableRequest request)
    {
        var timetable = await _context.Timetables
            .Include(t => t.Sessions)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (timetable == null)
        {
            return NotFound(new { message = "Timetable not found." });
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new
            {
                message = "Please give your timetable a name."
            });
        }

        timetable.Name = request.Name.Trim();
        timetable.Description = (request.Description ?? "").Trim();

        var inputs = request.Sessions ?? new List<StudySessionInput>();

        // Keep existing sessions and their completion status when their IDs match.
        var existingSessions = timetable.Sessions.ToDictionary(s => s.Id);
        var retainedIds = inputs
            .Where(input => input.Id > 0 && existingSessions.ContainsKey(input.Id))
            .Select(input => input.Id)
            .ToHashSet();

        // A session with ID 0 is a new session, not a reason to keep old sessions.
        var removedSessions = timetable.Sessions
            .Where(s => !retainedIds.Contains(s.Id))
            .ToList();

        _context.StudySessions.RemoveRange(removedSessions);

        foreach (var input in inputs)
        {
            if (input.Id > 0 && existingSessions.TryGetValue(input.Id, out var existing))
            {
                existing.Date = input.Date ?? "";
                existing.StartTime = input.StartTime ?? "";
                existing.Minutes = input.Minutes;
                existing.Subject = input.Subject ?? "";
                existing.Topic = input.Topic ?? "";
                existing.Notes = input.Notes ?? "";
            }
            else
            {
                timetable.Sessions.Add(ToSession(input));
            }
        }

        await _context.SaveChangesAsync();

        return Ok(timetable);
    }

    // PUT /api/timetables/5/sessions/12
    [HttpPut("{id:int}/sessions/{sessionId:int}")]
    public async Task<ActionResult<StudySession>> UpdateSession(
        int id,
        int sessionId,
        [FromBody] UpdateSessionRequest request)
    {
        var session = await _context.StudySessions
            .FirstOrDefaultAsync(s =>
                s.Id == sessionId && s.TimetableId == id);

        if (session == null)
        {
            return NotFound(new { message = "Study session not found." });
        }

        session.IsComplete = request.IsComplete;

        await _context.SaveChangesAsync();

        return Ok(session);
    }

    // GET /api/timetables/5/progress
    [HttpGet("{id:int}/progress")]
    public async Task<ActionResult<TimetableProgress>> GetProgress(int id)
    {
        var timetable = await _context.Timetables
            .Include(t => t.Sessions)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (timetable == null)
        {
            return NotFound(new { message = "Timetable not found." });
        }

        return Ok(BuildProgress(timetable));
    }

    // DELETE /api/timetables/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTimetable(int id)
    {
        var timetable = await _context.Timetables
            .FirstOrDefaultAsync(t => t.Id == id);

        if (timetable == null)
        {
            return NotFound(new { message = "Timetable not found." });
        }

        _context.Timetables.Remove(timetable);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Timetable deleted." });
    }

    private static StudySession ToSession(StudySessionInput input)
    {
        return new StudySession
        {
            Date = input.Date ?? "",
            StartTime = input.StartTime ?? "",
            Minutes = input.Minutes,
            Subject = input.Subject ?? "",
            Topic = input.Topic ?? "",
            Notes = input.Notes ?? "",
            IsComplete = false
        };
    }

    private static TimetableProgress BuildProgress(Timetable timetable)
    {
        var sessions = timetable.Sessions;

        var progress = new TimetableProgress
        {
            TotalSessions = sessions.Count,
            CompletedSessions = sessions.Count(s => s.IsComplete),
            PlannedMinutes = sessions.Sum(s => s.Minutes),
            CompletedMinutes = sessions
                .Where(s => s.IsComplete)
                .Sum(s => s.Minutes)
        };

        progress.Percentage = progress.TotalSessions == 0
            ? 0
            : (int)Math.Round(
                progress.CompletedSessions * 100.0 / progress.TotalSessions);

        progress.PerSubject = sessions
            .GroupBy(s => s.Subject)
            .Select(group => new SubjectProgress
            {
                Subject = group.Key,
                Total = group.Count(),
                Completed = group.Count(s => s.IsComplete),
                Percentage = group.Count() == 0
                    ? 0
                    : (int)Math.Round(
                        group.Count(s => s.IsComplete) * 100.0 / group.Count())
            })
            .OrderBy(p => p.Subject)
            .ToList();

        return progress;
    }
}
