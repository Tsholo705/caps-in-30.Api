namespace caps_in_30.Api.Models
{
    public class AccessCode
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public int Grade { get; set; }

        public bool IsUsed { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UsedAt { get; set; }
    }
}