namespace RaceDay.API.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Distance { get; set; }
        public decimal EntryFee { get; set; }
        public int EventId { get; set; }
    }
}