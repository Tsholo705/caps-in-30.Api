namespace caps_in_30.Api.Models
{
    public class Score
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public string Subject { get; set; }

        public string Topic { get; set; }

        public int Grade { get; set; }

        public int Correct { get; set; }

        public int Total { get; set; }

        public int Percentage { get; set; }

        public int TimeUsed { get; set; }

        public DateTime Date { get; set; }
    }
}