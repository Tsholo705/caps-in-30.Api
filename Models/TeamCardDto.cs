namespace caps_in_30.Api.Models
{
    public class TeamCardDto
    {
        public int Id { get; set; }

        public int Grade { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string Topic { get; set; } = string.Empty;

        public string Term { get; set; } = string.Empty;

        public List<string> ForbiddenWords { get; set; } = new();

        public int Points { get; set; }
    }
}