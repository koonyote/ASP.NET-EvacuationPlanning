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
        public required double Latitude { get; set; }
        [Required]
        public required double Longitude { get; set; }
        [Required]
        public required double Speed { get; set; }
    }
}
