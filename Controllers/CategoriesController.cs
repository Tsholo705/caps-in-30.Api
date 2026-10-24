using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using caps_in_30.Api.Data;
using caps_in_30.Api.Models;

namespace caps_in_30.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/categories/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found."
                });
            }

            return Ok(category);
        }

        // GET: api/categories?subjectId=2
        [HttpGet("by-subject/{subjectId}")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesBySubject(int subjectId)
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.SubjectId == subjectId)
                .ToListAsync();

            return Ok(categories);
        }

        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(Category category)
        {
            var subjectExists = await _context.Subjects
                .AnyAsync(s => s.Id == category.SubjectId);

            if (!subjectExists)
            {
                return BadRequest(new
                {
                    message = "The specified SubjectId does not exist."
                });
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.Id },
                category
            );
        }

        
        // GET: api/categories/by-subject/7/grade/3
        [HttpGet("by-subject/{subjectId}/grade/{grade}")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesBySubjectAndGrade(
            int subjectId,
            int grade)
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c =>
                    c.SubjectId == subjectId &&
                    (
                    _context.Questions.Any(q =>
                        q.CategoryId == c.Id &&
                        q.SubjectId == subjectId &&
                        q.Grade == grade)
                    ||
                    _context.SpellingWords.Any(w =>
                        w.CategoryId == c.Id &&
                        w.SubjectId == subjectId &&
                        w.Grade == grade)
                ))
                .ToListAsync();

            return Ok(categories);
        }

    }
}