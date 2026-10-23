
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using caps_in_30.Api.Data;
using System.Text.Json;
using caps_in_30.Api.Models;

namespace caps_in_30.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpellingWordsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SpellingWordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/spellingwords?grade=3&subjectId=7&categoryId=21
        [HttpGet]
        public async Task<IActionResult> GetSpellingWords(
            int grade,
            int subjectId,
            int categoryId)
        {
            var words = await _context.SpellingWords
                .Where(w =>
                    w.Grade == grade &&
                    w.SubjectId == subjectId &&
                    w.CategoryId == categoryId)
                .OrderBy(w => w.Id)
                .Select(w => new
                {
                    w.Id
                })
                .ToListAsync();

            if (words.Count == 0)
            {
                return NotFound("No spelling words found for these selections.");
            }

            return Ok(words);
        }


        // POST: api/spellingwords/1/check
        [HttpPost("{id:int}/check")]
        public async Task<IActionResult> CheckSpelling(
            int id,
            [FromBody] CheckSpellingDto request)
        {
            var spellingWord = await _context.SpellingWords
                .FirstOrDefaultAsync(w => w.Id == id);

            if (spellingWord == null)
            {
                return NotFound("Spelling word not found.");
            }

            bool isCorrect = string.Equals(
                spellingWord.Word.Trim(),
                request.Answer?.Trim(),
                StringComparison.OrdinalIgnoreCase);

            return Ok(new
            {
                isCorrect
            });
        }

    }
}
