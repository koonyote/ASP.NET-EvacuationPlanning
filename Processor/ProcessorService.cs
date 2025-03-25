using EvacuationPlanning.Processor.Dto;
using EvacuationPlanning.Vehicles.Dto;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;

namespace EvacuationPlanning.Processor
{
    public class ProcessorService
    {
        private readonly IDatabase _db;
        private readonly IConnectionMultiplexer _context;
        private readonly IConfiguration _configuration;
        private readonly string _key = "Transports";

        public ProcessorService(
            IConnectionMultiplexer mux,
            //IServiceCollection services
            IConfiguration configuration
            )
        {
            _db = mux.GetDatabase();
            _context = mux;
            _configuration = configuration;
        }

        public async Task<string> ClearAllAsync()
        {
            var endPoint = _configuration["ConnectionStrings:EndPoint"];
            if (endPoint == null) throw new ArgumentNullException("EndPoint");
            await _context.GetServer(endPoint).FlushDatabaseAsync();
            return "Deleted successfully!";
        }

        public async Task SaveTransportAsync(TransportDto input)
        {
            var jsonString = JsonSerializer.Serialize(input);

            await _db.HashSetAsync(_key, input.Id, jsonString);
        }

        public async Task<TransportDto> GetOneTransportAsync(string transportId)
        {
            var transportJson = await _db.HashGetAsync(_key, transportId);

            if (!transportJson.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<TransportDto>(transportJson);
            }
            else throw new InvalidDataException("Can't find transport logs.");
        }

        public async Task<List<TransportDto>> GetAllTransportAsync()
        {
            // ค้นหาคีย์ทั้งหมดใน Hash
            var keys = await _db.HashKeysAsync(_key);

            var transports = new List<TransportDto>();
            foreach (var key in keys)
            {
                // ดึง JSON String ของแต่ละ Transport
                var transportJson = await _db.HashGetAsync("Transports", key);
                if (!transportJson.IsNullOrEmpty)
                {
                    // Deserialize JSON String เป็น Object
                    var transport = JsonSerializer.Deserialize<TransportDto>(transportJson);
                    transports.Add(transport);
                }
            }

            return transports;
        }
    }
}
