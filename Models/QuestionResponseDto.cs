using System.Text.Json;

namespace caps_in_30.Api.Models
{
    public class QuestionResponseDto
    {
        public int Id { get; set; }

        public int Grade { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string Topic { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public List<string> Options { get; set; } = new();

        public int CorrectIndex { get; set; }
    }
}