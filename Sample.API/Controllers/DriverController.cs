using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sample.API.Data;
using Sample.API.Models;
using Sample.API.Services;

namespace Sample.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DriverController : ControllerBase
    {
        private readonly ILogger<DriverController> _logger;
        private readonly ICacheService _cacheService;
        private readonly AppDbContext _context;

        public DriverController(ILogger<DriverController> logger, ICacheService cacheService, AppDbContext context)
        {
            _logger = logger;
            _cacheService = cacheService;
            _context = context;
        }

        [HttpGet("getdrivers")]
        public async Task<IActionResult> GetDrivers()
        {
            try
            {
                var cacheData = _cacheService.Get<IEnumerable<Driver>>("drivers");

                if (cacheData != null)
                {
                    return Ok(cacheData);
                }

                cacheData = await _context.Drivers.ToListAsync();
                _cacheService.Set("drivers", cacheData, DateTimeOffset.Now.AddMinutes(1));

                return Ok(cacheData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting drivers");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost("adddrivers")]
        public async Task<IActionResult> AddDriver(Driver driver)
        {
            try
            {
                var addedDriver = await _context.Drivers.AddAsync(driver);
                await _context.SaveChangesAsync();
                _cacheService.Set($"driver{addedDriver.Entity.Id}", addedDriver.Entity, DateTimeOffset.Now.AddMinutes(1));

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a driver");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpDelete("deletedriver")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            try
            {
                var driver = await _context.Drivers.FirstOrDefaultAsync(x => x.Id == id);

                if (driver == null)
                {
                    return NotFound();
                }

                _context.Drivers.Remove(driver);
                await _context.SaveChangesAsync();
                _cacheService.Remove($"driver{id}");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting a driver");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}
