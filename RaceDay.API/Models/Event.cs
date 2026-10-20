namespace RaceDay.API.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;

        // New requirements from Part 2 brief
        public double Distance { get; set; }
        public string EventType { get; set; } = string.Empty; // "Run", "Walk", "Cycle"

        public int OrganiserId { get; set; }
        public User? Organiser { get; set; }

        // Navigation properties
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Route> Routes { get; set; } = new List<Route>();
        public ICollection<Weather> Weathers { get; set; } = new List<Weather>();
    }
}