using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Distance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal EntryFee { get; set; }

        public int EventId { get; set; }
        public Event? Event { get; set; }

        // Navigation property
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}