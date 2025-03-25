namespace EvacuationPlanning.Processor.Dto
{
    public class GetEvacuationStatusDto
    {
        public required string ZoneId { get; set;}
        public required int TotalEvacuated { get; set;}
        public required int RemainingPeople { get; set;}
        public string? LastVehicleUsed { get; set;}
    }
}
