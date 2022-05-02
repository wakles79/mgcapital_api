using MGCap.Business.Abstract.ApplicationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class DirectoryController : Controller
    {
        private readonly IDirectoryApplicationService _directoryApplicationService;

        public DirectoryController(IDirectoryApplicationService directoryApplicationService) 
        {
            _directoryApplicationService = directoryApplicationService;
        }

        [HttpGet]
        public async Task<IActionResult> ReadAll()
        {
            try
            {
                var result = await _directoryApplicationService.ReadAllAsync();
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return NoContent();
            }
        }

        [HttpGet]
        public async Task<IActionResult> EmployeeDetails(int employeeId)
        {
            try
            {
                var result = await _directoryApplicationService.EmployeeDetails(employeeId);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return NoContent();
            }
        }

        [HttpGet]
        public async Task<IActionResult> BuildingDetails(int buildingId)
        {
            try
            {
                var result = await _directoryApplicationService.BuildingDetails(buildingId);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return NoContent();
            }
        }

        [HttpGet]
        public async Task<IActionResult> CustomerDetails(int customerId)
        {
            try
            {
                var result = await _directoryApplicationService.CustomerDetails(customerId);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return NoContent();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ContactDetails(int contactId)
        {
            try
            {
                var result = await _directoryApplicationService.ContactDetails(contactId);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return NoContent();
            }
        }
    }
}