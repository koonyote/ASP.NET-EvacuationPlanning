using EvacuationPlanning.Vehicles.Dto;

namespace EvacuationPlanning.ApplicationShare
{
    public class VehicleDistance
    {
        public required VehicleDto Vehicle { get; set; }
        public required double Distance { get; set; }
        public required double ETA { get; set; }
    }
}
