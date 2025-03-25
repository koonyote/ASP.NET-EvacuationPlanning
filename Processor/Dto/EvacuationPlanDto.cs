using EvacuationPlanning.ApplicationShare;
using System.Security.Cryptography.X509Certificates;

namespace EvacuationPlanning.Processor.Dto
{
    public class EvacuationPlanDto
    {
        public required string ZoneID { get; set; }
        public required string VehicleID { get; set; }
        public required string ETA { get; set; }
        public required int NumberOfPeople { get; set; }

        
        // # Debug
        //public CoordinatesDto Vehicle { get; set; }
        //public double Speed { get; set; }
        //public CoordinatesDto Zone { get; set; } 
    }
}
