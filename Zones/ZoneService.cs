using EvacuationPlanning.Zones.Dto;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using StackExchange.Redis;
using System.Text.Json;

namespace EvacuationPlanning.Zones
{
    public class ZoneService
    {
        private readonly IDatabase _db;
        private readonly string _key = "Zones";

        public ZoneService(IConnectionMultiplexer mux)
        {
            _db = mux.GetDatabase();
        }

        public async Task SaveZoneAsync(ZoneDto input)
        {
            var jsonString = JsonSerializer.Serialize(input);

            await _db.HashSetAsync(_key, input.ZoneId, jsonString);
        }


        public async Task<ZoneDto> GetOneZoneAsync(string zoneId)
        {
            var zoneJson = await _db.HashGetAsync(_key, zoneId);

            if (!zoneJson.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<ZoneDto>(zoneJson);
            }
            else throw new InvalidDataException("Can't find zone");
        }

        public async Task<List<ZoneDto>> GetAllZoneAsync()
        {
            // ค้นหาคีย์ทั้งหมดใน Hash
            var keys = await _db.HashKeysAsync(_key);

            var zones = new List<ZoneDto>();
            foreach (var key in keys)
            {
                // ดึง JSON String ของแต่ละ Zone
                var zoneJson = await _db.HashGetAsync("Zones", key);
                if (!zoneJson.IsNullOrEmpty)
                {
                    // Deserialize JSON String เป็น Object
                    var zone = JsonSerializer.Deserialize<ZoneDto>(zoneJson);
                    zones.Add(zone);
                }
            }

            return zones;
        }
    }
}
