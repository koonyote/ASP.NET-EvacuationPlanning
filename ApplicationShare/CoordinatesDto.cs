using System.ComponentModel.DataAnnotations;

namespace EvacuationPlanning.ApplicationShare
{
    public class CoordinatesDto
    {
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
    }
}
