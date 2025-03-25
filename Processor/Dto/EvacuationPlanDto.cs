namespace EvacuationPlanning.Processor.Dto
{
    public class EvacuationPlanDto
    {
        public required string ZoneID { get; set; }
        public required string VehicleID { get; set; }
        public required string ETA { get; set; }
        public required int NumberOfPeople { get; set; }
    }
}
