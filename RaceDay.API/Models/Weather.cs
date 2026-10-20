namespace RaceDay.API.Models
{
    public class Weather
    {
        public int WeatherId { get; set; }
        public double Temperature { get; set; }
        public string Condition { get; set; } = string.Empty;
        public double WindSpeed { get; set; }

        public int EventId { get; set; }
        public Event? Event { get; set; }
    }
}