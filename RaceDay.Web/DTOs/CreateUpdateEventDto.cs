namespace RaceDay.API.DTOs
{
    public class CreateUpdateEventDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public double Distance { get; set; }
        public string EventType { get; set; } = string.Empty; // "Run", "Walk", "Cycle"
    }
}