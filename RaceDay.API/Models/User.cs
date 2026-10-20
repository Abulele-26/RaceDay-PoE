namespace RaceDay.API.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Organiser" or "Participant"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}