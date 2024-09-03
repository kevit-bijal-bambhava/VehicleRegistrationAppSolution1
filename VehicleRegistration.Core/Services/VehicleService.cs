﻿using Microsoft.EntityFrameworkCore;
using VehicleRegistration.Infrastructure.DataBaseModels;
using VehicleRegistration.Core.ServiceContracts;
using VehicleRegistration.Infrastructure;
using Microsoft.Extensions.Logging;
using Serilog;

namespace VehicleRegistration.Core.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<VehicleService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public VehicleService(ApplicationDbContext db, ILogger<VehicleService> logger, IDiagnosticContext diagnosticContext)
        {
            _db = db;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }
        public Task<VehicleModel> GetVehicleByIdAsync(Guid vehicleId)
        {
            _logger.LogInformation("WebAPI_GetVehicleById_VehicleService: " + vehicleId);
            return _db.VehiclesDetails.FindAsync(vehicleId).AsTask();
        }
        public async Task<IEnumerable<VehicleModel>> GetAllVehicles(string userId)
        {
            _logger.LogInformation("WebAPI_GetAllVehicles_VehicleService with UserID: " + userId);
            return await _db.VehiclesDetails
                                 .Where(v => v.UserId == int.Parse(userId))
                                 .ToListAsync();
        }
        public async Task<VehicleModel> AddVehicle(VehicleModel newVehicle)
        {
            _logger.LogInformation("WebAPI_AddVehicle_VehicleService");
            _db.VehiclesDetails.Add(newVehicle);
            await _db.SaveChangesAsync();
            _diagnosticContext.Set("NewVehicle", newVehicle);
            _logger.LogInformation("Vehicle Added Successfully.");
            return newVehicle;
        }
        public async Task<VehicleModel> EditVehicle(VehicleModel vehicle)
        {
            _logger.LogInformation("WebAPI_EditVehicle_VehicleService Called.");
            _db.VehiclesDetails.Update(vehicle);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Vehicle Details Updated.");
            return vehicle;
        }
        public async Task<VehicleModel> DeleteVehicle(Guid vehicleId)
        {
            _logger.LogInformation("WebAPI_DeleteVehicle_VehicleService Called.");
            var vehicle = await _db.VehiclesDetails.FindAsync(vehicleId);
            if (vehicle == null) return null;

            _db.VehiclesDetails.Remove(vehicle);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Vehicle Deleted Successfully with Id: " + vehicleId);
            return vehicle;
        }
    }
}
