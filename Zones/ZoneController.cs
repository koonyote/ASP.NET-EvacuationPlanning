using EvacuationPlanning.Zones.Dto;
using Microsoft.AspNetCore.Mvc;

namespace EvacuationPlanning.Zones
{
    [ApiController]
    //[Route("api/[controller]")]
    [Route("api")]
    public class ZoneController : ControllerBase
    {
        private readonly ZoneService _service;

        public ZoneController(ZoneService service)
        {
            _service = service;
        }

        [HttpPost("evacuation-zones")]
        public async Task<IActionResult> AddZone([FromBody] ZoneDto input)
        {
            await _service.SaveZoneAsync(input);
            return Ok("Added successfully!");
        }

        [HttpGet("Zones/GetAll")]
        public async Task<IActionResult> GetAllZone()
        {
            return Ok(await _service.GetAllZoneAsync());
        }

        [HttpGet("Zones/Get/{id}")]
        public async Task<IActionResult> GetZone(string id)
        {
            var zone = await _service.GetOneZoneAsync(id);
            return Ok(zone);
        }
    }
}
