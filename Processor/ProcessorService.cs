using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace EvacuationPlanning.Processor
{
    public class ProcessorService
    {
        private readonly IDatabase _db;
        private readonly IConnectionMultiplexer _context;
        private readonly IConfiguration _configuration;

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
    }
}
