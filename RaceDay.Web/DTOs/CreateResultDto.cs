namespace RaceDay.API.DTOs
{
    public class CreateResultDto
    {
        public int EnrollmentId { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int Position { get; set; }
    }
}