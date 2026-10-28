using System.Text.Json.Serialization;
namespace caps_in_30.Api.Models
{
    // One study session on a specific date.
    public class StudySession
    {
        public int Id { get; set; }

        public int TimetableId { get; set; }
        
        [JsonIgnore]
        public Timetable Timetable { get; set; } = null!;

        public string Date { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public int Minutes { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
    }

    // A study plan belonging to one learner.
    public class Timetable
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public List<StudySession> Sessions { get; set; } = new();
    }

    // A session submitted when creating or updating a timetable.
    public class StudySessionInput
    {
        public int Id { get; set; }
        public string Date { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public int Minutes { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    // Request to create a timetable.
    public class SaveTimetableRequest
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<StudySessionInput> Sessions { get; set; } = new();
    }

    // Request to update a timetable.
    public class UpdateTimetableRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<StudySessionInput> Sessions { get; set; } = new();
    }

    // Request to mark a session complete or incomplete.
    public class UpdateSessionRequest
    {
        public bool IsComplete { get; set; }
    }

    // Progress for an individual subject.
    public class SubjectProgress
    {
        public string Subject { get; set; } = string.Empty;
        public int Total { get; set; }
        public int Completed { get; set; }
        public int Percentage { get; set; }
    }

    // Overall timetable progress.
    public class TimetableProgress
    {
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public int PlannedMinutes { get; set; }
        public int CompletedMinutes { get; set; }
        public int Percentage { get; set; }

        public List<SubjectProgress> PerSubject { get; set; } = new();
    }
}
