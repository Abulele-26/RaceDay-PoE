namespace RaceDay.API.DTOs
{
    public class CreateUpdateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public double Distance { get; set; }
        public decimal EntryFee { get; set; }
    }
}