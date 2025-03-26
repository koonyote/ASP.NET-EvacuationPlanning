using EvacuationPlanning.ApplicationShare;
using EvacuationPlanning.Processor.Dto;
using EvacuationPlanning.Vehicles;
using EvacuationPlanning.Vehicles.Dto;
using EvacuationPlanning.Zones;
using EvacuationPlanning.Zones.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Validations;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace EvacuationPlanning.Processor
{
    [ApiController]
    [Route("api")]
    public class ProcessorController : ControllerBase
    {
        private readonly IZoneController _zone;
        private readonly IVehicleController _vehicle;
        private readonly ProcessorService _service;

        public ProcessorController(
            IZoneController zone,
            IVehicleController vehicle,
            ProcessorService service)
        {
            _zone = zone;
            _vehicle = vehicle;
            _service = service;
        }

        [HttpDelete("evacuations/clear")]
        public async Task<IActionResult> ClearAll()
        {
            return Ok(await _service.ClearAllAsync());
        }

        [HttpPost("evacuations/plan")]
        public async Task<List<EvacuationPlanDto>> PlanGenerator()
        {
            List<ZoneDto> zones = await _zone.GetAllZone();

            List<VehicleDto> vehicles = await _vehicle.GetAllVehicle();

            var result = new List<EvacuationPlanDto>();

            // เรียงลำดับการทำงานตาม Urgency Priority
            foreach (var zone in zones.OrderByDescending(o => o.UrgencyLevel))
            {
                if (vehicles.Count == 0) break; // หากรถหมดแล้ว ให้หยุดทำงาน 

                var logs = await _service.GetAllTransportAsync();

                int transferAmount = logs.Where(w => w.ZoneId == zone.ZoneId).Sum(s => s.Amount);

                if (transferAmount >= zone.NumberOfPeople) continue; // หากผู้คนอพยกหมดแล้ว ให้ข้ามไป

                // ค้นหารถที่ ใช้เวลาน้อยที่สุด
                var listVehicle = vehicles.Select(s => new
                {
                    info = FindVehicleDistance(s, zone.LocationCoordinates)
                })
                .OrderBy(o => o.info.ETA)
                .ThenBy(o => o.info.Distance)
                .ToArray();

                var selectVeh = listVehicle.First();

                // ค้นหารถ ลำดับที่สอง หากความจุมากกว่าหรือเท่ากับ 10 คน และใช้เวลาไม่มากกว่า 3 นาที ก็ให้ใช้คันนี้แทน (Capacity Optimization)
                //double maxMinute = 3, moreCapacity = 20;
                //var second = listVehicle.Where(w =>
                //( w.info.Vehicle.Capacity - selectVeh.info.Vehicle.Capacity) >= moreCapacity
                //&& (w.info.ETA - selectVeh.info.ETA) <= maxMinute)
                //.OrderBy(o => o.info.Vehicle.Capacity).ThenBy(o => o.info.ETA);

                //if (second.Any()) selectVeh = second.First();

                result.Add(new EvacuationPlanDto()
                {
                    ZoneID = zone.ZoneId,
                    VehicleID = selectVeh.info.Vehicle.VehicleId,
                    ETA = selectVeh.info.ETA + " minutes",
                    NumberOfPeople = selectVeh.info.Vehicle.Capacity

                    //# Debug
                    //, Speed = selectVeh.info.Vehicle.Speed,
                    //Vehicle = selectVeh.info.Vehicle.LocationCoordinates,
                    //Zone = zone.LocationCoordinates,
                });

                vehicles.Remove(selectVeh.info.Vehicle);
            }

            return result;
        }

        static double CalculateEstimatedTimeOfArrival(double distance, double speed) => (distance / speed) * 60; // แปลงชั่วโมงเป็นนาที

        private static VehicleDistance FindVehicleDistance(VehicleDto vehicle, CoordinatesDto zone)
        {
            double distance = HaversineFormula.Haversine(zone.Latitude, zone.Longitude,
                        vehicle.LocationCoordinates.Latitude, vehicle.LocationCoordinates.Longitude);

            double eta = CalculateEstimatedTimeOfArrival(distance, vehicle.Speed);

            return new VehicleDistance()
            {
                Vehicle = vehicle,
                Distance = distance,
                ETA = Math.Round(eta, 0)
            };
        }


        [HttpPost("evacuations/update")]
        public async Task AssignTransport(string zoneId, string vehicleId, int amount)
        {
            var zone = await _zone.GetZone(zoneId);

            var vehicle = await _vehicle.GetVehicle(vehicleId);

            var distance = FindVehicleDistance(vehicle, zone.LocationCoordinates);

            var logs = await _service.GetAllTransportAsync();

            int transferAmount = logs.Where(w => w.ZoneId == zone.ZoneId).Sum(s => s.Amount);

            string completed = "C", inProcess = "I";

            string status = ((transferAmount + amount) >= zone.NumberOfPeople) ? completed : inProcess;

            var transport = new TransportDto()
            {
                Id = DateTime.Now.ToString("yyyyMMddHHmmss"),
                ZoneId = zoneId,
                VehicleId = vehicleId,
                Amount = amount,
                RemainingPeople = (status == completed) ? 0 : zone.NumberOfPeople - (transferAmount + amount),
                Speed = vehicle.Speed,
                Distance = Math.Round(distance.Distance, 2),
                Unit = "KM.",
                ETA = distance.ETA + " minutes",
                Progress = status,

                VehicleCoordinates = vehicle.LocationCoordinates,
                ZoneCoordinates = zone.LocationCoordinates,
            };

            await _service.SaveTransportAsync(transport);
        }

        [HttpPost("evacuations/status")]
        public async Task<GetEvacuationStatusDto[]> GetEvacuationStatus()
        {
            var zones = await _zone.GetAllZone();

            var transports = await _service.GetAllTransportAsync();

            var result = new List<GetEvacuationStatusDto>();

            foreach (var zone in zones)
            {
                var query = transports.Where(w => w.ZoneId == zone.ZoneId).OrderBy(o => o.Id);

                int transferAmount = transports.Where(w => w.ZoneId == zone.ZoneId).Sum(s => s.Amount);

                string? lastVehicleUsed = (query.Any()) ? query.Last().VehicleId : null;

                string? operationStatus = (query.Any()) ? query.Last().Progress : null;

                result.Add(new GetEvacuationStatusDto()
                {
                    ZoneId = zone.ZoneId,
                    TotalEvacuated = zone.NumberOfPeople,
                    RemainingPeople = (zone.NumberOfPeople - transferAmount < 0) ? 0 : zone.NumberOfPeople - transferAmount,
                    LastVehicleUsed = lastVehicleUsed,
                    OperationStatus = operationStatus,
                });
            }

            return result.OrderBy(o => o.ZoneId).ToArray();
        }

        [HttpPost("GetAllTransportLogs")]
        public async Task<List<TransportDto>> GetAllTransportAsync()
        {
            return await _service.GetAllTransportAsync();
        }






        //[HttpGet("Haversine")]
        //public double HaversineCal(double lat1, double lon1, double lat2, double lon2) => HaversineFormula.Haversine(lat1, lon1, lat2, lon2);

        //[HttpGet("HaversineCalculateEstimatedTimeOfArrival")]
        //public double HaversineCalculateEstimatedTimeOfArrival(double lat1, double lon1, double lat2, double lon2, double speed) => (HaversineFormula.Haversine(lat1, lon1, lat2, lon2) / speed) * 60;

        // #Initial Data 
        [HttpPost("Initial")]
        public async Task Initial()
        {
            var vehicles = new List<VehicleDto>()
            {
                new VehicleDto()
                {
                    VehicleId = "V1",
                    Capacity = 40,
                    Type = "bus",
                    LocationCoordinates = new ApplicationShare.CoordinatesDto()
                    {
                        Latitude = 13.7650,
                        Longitude = 100.5381
                    },
                    Speed = 60
                },
                new VehicleDto()
                {
                    VehicleId = "V2",
                    Capacity = 20,
                    Type = "van",
                    LocationCoordinates = new ApplicationShare.CoordinatesDto()
                    {
                        Latitude = 13.7320,
                        Longitude = 100.5200
                    },
                    Speed = 50
                },
            };


            foreach (var vehicle in vehicles)
            {
                await _vehicle.AddVehicle(vehicle);
            }

            var zones = new List<ZoneDto>()
            {
                new ZoneDto()
                {
                    ZoneId = "Z1",
                    LocationCoordinates = new ApplicationShare.CoordinatesDto()
                    {
                        Latitude = 13.7563,
                        Longitude = 100.5018,
                    },
                    NumberOfPeople = 100,
                    UrgencyLevel = UrgencyLevel.High,
                },
                new ZoneDto()
                {
                    ZoneId = "Z2",
                    LocationCoordinates = new ApplicationShare.CoordinatesDto()
                    {
                        Latitude = 13.7367,
                        Longitude = 100.5231,
                    },
                    NumberOfPeople = 50,
                    UrgencyLevel = UrgencyLevel.VeryHigh,
                }
            };

            foreach (var zone in zones)
            {
                await _zone.AddZone(zone);
            }
        }

        [HttpPost("Test")]
        private async Task Test()
        {
            var vehicles = new List<VehicleDto>()
            {
                new VehicleDto()
                {
                    VehicleId = "V3",
                    Capacity = 50,
                    Type = "Truck",
                    LocationCoordinates = new ApplicationShare.CoordinatesDto()
                    {
                        Latitude = 13.8450,
                        Longitude = 101.0000
                    },
                    Speed = 70
                },
                new VehicleDto()
                {
                    VehicleId = "V4",
                    Capacity = 100,
                    Type = "Bus",
                    LocationCoordinates = new ApplicationShare.CoordinatesDto()
                    {
                        Latitude = 13.9999,
                        Longitude = 101.0101
                    },
                    Speed = 60
                },
            };

            foreach (var vehicle in vehicles)
            {
                await _vehicle.AddVehicle(vehicle);
            }

            var zones = new List<ZoneDto>()
            {
                new ZoneDto()
                {
                    ZoneId = "Z3",
                    LocationCoordinates = new ApplicationShare.CoordinatesDto()
                    {
                        Latitude =14.0123,
                        Longitude = 101.1234,
                    },
                    NumberOfPeople = 80,
                    UrgencyLevel = UrgencyLevel.Medium,
                },
                new ZoneDto()
                {
                    ZoneId = "Z4",
                    LocationCoordinates = new ApplicationShare.CoordinatesDto()
                    {
                        Latitude = 13.8765,
                        Longitude = 100.9876,
                    },
                    NumberOfPeople = 120,
                    UrgencyLevel = UrgencyLevel.High,
                }
            };

            foreach (var zone in zones)
            {
                await _zone.AddZone(zone);
            }
        }
    }
}
