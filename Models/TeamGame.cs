namespace caps_in_30.Api.Models
{
    public class TeamGame
    {
        public int Id { get; set; }

        public int Grade { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string Topic { get; set; } = string.Empty;

        public int NumberOfTeams { get; set; }

        public int NumberOfRounds { get; set; }

        public int CurrentTeam { get; set; }

        public int CurrentRound { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsFinished { get; set; }
    }
}