using EvacuationPlanning.Zones.Dto;
using Microsoft.AspNetCore.Mvc;

namespace EvacuationPlanning.Zones
{
    [ApiController]
    //[Route("api/[controller]")]
    [Route("api")]
    public class ZoneController : ControllerBase, IZoneController
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
        public async Task<List<ZoneDto>> GetAllZone()
        {
            return await _service.GetAllZoneAsync();
        }

        [HttpGet("Zones/Get/{id}")]
        public async Task<ZoneDto> GetZone(string id)
        {
            return await _service.GetOneZoneAsync(id);
        }
    }
}
