using Microsoft.AspNetCore.Mvc;

namespace EvacuationPlanning.Zones
{
    public interface IZoneController
    {
        Task<IActionResult> GetAllZone();
        Task<IActionResult> GetZone(string id);
    }
}
