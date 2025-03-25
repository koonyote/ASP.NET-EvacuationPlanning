using EvacuationPlanning.Vehicles.Dto;
using StackExchange.Redis;
using System.Text.Json;

namespace EvacuationPlanning.Vehicles
{
    public class VehicleService
    {
        private readonly IDatabase _db;
        private readonly string _key = "Vehicles";

        public VehicleService(IConnectionMultiplexer mux)
        {
            _db = mux.GetDatabase();
        }

        public async Task SaveVehicleAsync(VehicleDto input)
        {
            var jsonString = JsonSerializer.Serialize(input);

            await _db.HashSetAsync(_key, input.VehicleId, jsonString);
        }


        public async Task<VehicleDto> GetOneVehicleAsync(string vehicleId)
        {
            var vehicleJson = await _db.HashGetAsync(_key, vehicleId);

            if (!vehicleJson.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<VehicleDto>(vehicleJson);
            }
            else throw new InvalidDataException("Can't find vehicle");
        }

        public async Task<List<VehicleDto>> GetAllVehicleAsync()
        {
            // ค้นหาคีย์ทั้งหมดใน Hash
            var keys = await _db.HashKeysAsync(_key);

            var vehicles = new List<VehicleDto>();
            foreach (var key in keys)
            {
                // ดึง JSON String ของแต่ละ Vehicle
                var vehicleJson = await _db.HashGetAsync("Vehicles", key);
                if (!vehicleJson.IsNullOrEmpty)
                {
                    // Deserialize JSON String เป็น Object
                    var vehicle = JsonSerializer.Deserialize<VehicleDto>(vehicleJson);
                    vehicles.Add(vehicle);
                }
            }

            return vehicles;
        }
    }
}
