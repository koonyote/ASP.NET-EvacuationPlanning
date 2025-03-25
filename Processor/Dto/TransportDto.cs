namespace EvacuationPlanning.Processor.Dto
{
    public class TransportDto
    {
        public required string Id { get; set; }
        public required string ZoneID { get; set; }
        public required string VehicleID { get; set; }
        public int Amount { get; set; }
    }
}
