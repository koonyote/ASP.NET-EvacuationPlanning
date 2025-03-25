using Microsoft.AspNetCore.Mvc;
using EvacuationPlanning.Vehicles.Dto;

namespace EvacuationPlanning.Vehicles
{
    [ApiController]
    //[Route("api/[controller]")]
    [Route("api")]
    public class VehicleController : ControllerBase
    {
        private readonly VehicleService _service;

        public VehicleController(VehicleService service)
        {
            _service = service;
        }

        [HttpPost("vehicles")]
        public async Task<IActionResult> AddVehicle([FromBody] VehicleDto input)
        {
            await _service.SaveVehicleAsync(input);
            return Ok("Added successfully!");
        }

        [HttpGet("Vehicles/GetAll")]
        public async Task<IActionResult> GetAllVehicle()
        {
            return Ok(await _service.GetAllVehicleAsync());
        }

        [HttpGet("Vehicle/Get/{id}")]
        public async Task<IActionResult> GetVehicle(string id)
        {
            var vehicle = await _service.GetOneVehicleAsync(id);
            return Ok(vehicle);
        }
    }
}
