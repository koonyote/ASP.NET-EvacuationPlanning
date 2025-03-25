using EvacuationPlanning.Vehicles.Dto;
using Microsoft.AspNetCore.Mvc;

namespace EvacuationPlanning.Vehicles
{
    public interface IVehicleController
    {
        Task<IActionResult> AddVehicle(VehicleDto input);
        Task<List<VehicleDto>> GetAllVehicle();
        Task<VehicleDto> GetVehicle(string id);
    }
}
