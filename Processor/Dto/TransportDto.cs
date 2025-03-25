namespace EvacuationPlanning.Processor.Dto
{
    public class TransportDto
    {
        public required string Id { get; set; }
        public required string ZoneId { get; set; }
        public required string VehicleId { get; set; }
        public int Amount { get; set; }
    }
}
