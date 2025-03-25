using EvacuationPlanning.ApplicationShare;
using System.ComponentModel.DataAnnotations;

namespace EvacuationPlanning.Vehicles.Dto
{
    public class VehicleDto
    {
        [Required]
        public required string VehicleId { get; set; }
        [Required]
        public required int Capacity { get; set; }
        [Required]
        public required string Type { get; set; }
        [Required]
        public required CoordinatesDto LocationCoordinates { get; set; }
        [Required]
        public required double Speed { get; set; }
    }
}
