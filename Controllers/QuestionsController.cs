using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using caps_in_30.Api.Data;
using caps_in_30.Api.Models;

namespace caps_in_30.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuestionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/questions
        [HttpGet]
        public async Task<IActionResult> GetQuestions(
            int grade,
            string subject,
            string topic,
            int howMany = 10)
        {
            if (grade < 7 || grade > 8)
            {
                return BadRequest("Grade must be 7 or 8.");
            }

            if (howMany <= 0)
            {
                return BadRequest("howMany must be greater than 0.");
            }

            var questions = await _context.Questions
                .Include(q => q.Subject)
                .Include(q => q.Category)
                .Where(q =>
                    q.Grade == grade &&
                    q.Subject.Name == subject &&
                    q.Category.Name == topic &&
                    q.Category.SubjectId == q.SubjectId
                )
                .ToListAsync();

            if (!questions.Any())
            {
                return NotFound("No questions found for the selected grade, subject and topic.");
            }

            var random = new Random();

            var selectedQuestions = questions
                .OrderBy(q => random.Next())
                .Take(howMany)
                .Select(q => new QuestionResponseDto
                {
                    Id = q.Id,
                    Grade = q.Grade,
                    Subject = q.Subject.Name,
                    Topic = q.Category.Name,
                    Text = q.QuestionText,
                    Options = JsonSerializer.Deserialize<List<string>>(q.Options)
                               ?? new List<string>(),
                    CorrectIndex = q.CorrectIndex
                })
                .ToList();

            return Ok(selectedQuestions);
        }
    }
}