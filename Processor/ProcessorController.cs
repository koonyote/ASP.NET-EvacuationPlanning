using EvacuationPlanning.Vehicles;
using EvacuationPlanning.Zones;
using Microsoft.AspNetCore.Mvc;

namespace EvacuationPlanning.Processor
{
    [ApiController]
    [Route("api")]
    public class ProcessorController : ControllerBase
    {
        private readonly IZoneController _zone;
        private readonly IVehicleController _vehicle;
        private readonly ProcessorService _service;

        public ProcessorController(
            IZoneController zone,
            IVehicleController vehicle,
            ProcessorService service)
        {
            _zone = zone;
            _vehicle = vehicle;
            _service = service;
        }

        [HttpDelete("evacuations/clear")]
        public async Task<IActionResult> ClearAll()
        {
            return Ok(await _service.ClearAllAsync());
        }
    }
}
