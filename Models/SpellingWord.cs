
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace caps_in_30.Api.Models
{
    public class SpellingWord
    {
        public int Id { get; set; }

        [Required]
        public int Grade { get; set; }

        [Required]
        public string Word { get; set; } = string.Empty;

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public Subject? Subject { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }
    }
}
