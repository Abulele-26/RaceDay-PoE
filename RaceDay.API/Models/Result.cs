namespace RaceDay.API.Models
{
    public class Result
    {
        public int ResultId { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int Position { get; set; }

        public int EnrollmentId { get; set; }
        public Enrollment? Enrollment { get; set; }
    }
}