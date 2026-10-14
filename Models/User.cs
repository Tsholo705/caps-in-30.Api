namespace caps_in_30.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Maps to Name
        public string Username { get; set; } = string.Empty; // Used for login identification
        public string PasswordHash { get; set; } = string.Empty;
        public int Grade { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
}
