using System.ComponentModel.DataAnnotations;

namespace EvacuationPlanning.Zones.Dto
{
    public class ZoneDto
    {
        [Required]
        public required string ZoneId { get; set; }
        [Required]
        public required double Latitude { get; set; }
        [Required]
        public required double Longitude { get; set; }
        public int NumberOfPeople { get; set; }
        public UrgencyLevel UrgencyLevel { get; set; }
    }

    public enum UrgencyLevel
    {
        VeryLow,
        Low,
        Medium,
        High,
        VeryHigh,
    }
}
