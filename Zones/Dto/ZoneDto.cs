using EvacuationPlanning.ApplicationShare;
using System.ComponentModel.DataAnnotations;

namespace EvacuationPlanning.Zones.Dto
{
    public class ZoneDto
    {
        [Required]
        public required string ZoneId { get; set; }
        [Required]
        public required CoordinatesDto LocationCoordinates { get; set; }
        public int NumberOfPeople { get; set; }
        public UrgencyLevel UrgencyLevel { get; set; }
    }

    public enum UrgencyLevel
    {
        None,
        VeryLow,
        Low,
        Medium,
        High,
        VeryHigh,
    }
}
