using System.Text.Json.Serialization;

namespace caps_in_30.Api.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int SubjectId { get; set; }

        [JsonIgnore]
        public Subject Subject { get; set; } = null!;

        [JsonIgnore]
        public List<Question> Questions { get; set; } = new();
    }
}