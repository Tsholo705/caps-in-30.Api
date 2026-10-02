using System.Text.Json.Serialization;

namespace caps_in_30.Api.Models
{
    public class Question
    {
        public int Id { get; set; }

        public int Grade { get; set; }

        public int SubjectId { get; set; }

        public int CategoryId { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public string Options { get; set; } = string.Empty;

        public int CorrectIndex { get; set; }

        [JsonIgnore]
        public Subject Subject { get; set; } = null!;

        [JsonIgnore]
        public Category Category { get; set; } = null!;
    }
}