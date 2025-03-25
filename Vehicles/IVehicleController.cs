using Microsoft.AspNetCore.Mvc;

namespace EvacuationPlanning.Vehicles
{
    public interface IVehicleController
    {
        Task<IActionResult> GetAllVehicle();
        Task<IActionResult> GetVehicle(string id);
    }
}
