namespace RaceDay.API.DTOs
{
    public class EnrollmentDto
    {
        public int EnrollmentId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }
}