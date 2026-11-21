namespace RaceDay.API.DTOs
{
    public class ResultDto
    {
        public int ResultId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string FinishTime { get; set; } = string.Empty;
        public int Position { get; set; }
    }
}