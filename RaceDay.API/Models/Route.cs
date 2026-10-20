namespace RaceDay.API.Models
{
    public class Route
    {
        public int RouteId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public string MapUrl { get; set; } = string.Empty;
        public string TerrainType { get; set; } = string.Empty;

        public int EventId { get; set; }
        public Event? Event { get; set; }
    }
}