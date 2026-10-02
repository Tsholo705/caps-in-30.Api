using System.Text.Json.Serialization;

namespace caps_in_30.Api.Models
{
    public class TeamCard
    {
        public int Id { get; set; }

        public int Grade { get; set; }

        public int SubjectId { get; set; }

        public int CategoryId { get; set; }

        public string Term { get; set; } = string.Empty;

        // Stored as JSON in SQL Server
        public string ForbiddenWords { get; set; } = string.Empty;

        public int Points { get; set; }

        [JsonIgnore]
        public Subject Subject { get; set; } = null!;

        [JsonIgnore]
        public Category Category { get; set; } = null!;
    }
}