namespace EvacuationPlanning.Processor.Dto
{
    public class TransportDto
    {
        public required string Id { get; set; }
        public required string ZoneId { get; set; }
        public required string VehicleId { get; set; }
        public int Amount { get; set; }
        public required double Speed { get; set; }
        public required double RemainingPeople { get; set; }
        public required double Distance { get; set; }
        public required string Unit { get; set; }
        public required string ETA { get; set; }
        public required string Progress { get; set; }
    }
}
