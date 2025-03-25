using EvacuationPlanning.Zones.Dto;
using Microsoft.AspNetCore.Mvc;

namespace EvacuationPlanning.Zones
{
    public interface IZoneController
    {
        Task<IActionResult> AddZone(ZoneDto input);
        Task<List<ZoneDto>> GetAllZone();
        Task<IActionResult> GetZone(string id);
    }
}
