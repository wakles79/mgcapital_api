using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.Mobile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class MobileAppVersionController : Controller
    {
        private readonly IMapper Mapper;

        private readonly IMobileAppVersionService Service;

        public MobileAppVersionController(IMobileAppVersionService service, IMapper mapper)
        {
            Mapper = mapper;
            Service = service;
        }

        /// <summary>
        /// Updates the latest version for a given platform (ios, android) and mobile app (manager, client).
        /// </summary>
        /// <returns>The version.</returns>
        /// <param name="viewModel">View model.</param>
        [HttpPost]
        public async Task<ActionResult<MobileAppVersion>> AddVersion([FromBody] MobileAppVersionViewModel viewModel)
        {
            try
            {
                var obj = Mapper.Map<MobileAppVersionViewModel, MobileAppVersion>(viewModel);
                
                obj = await Service.AddAsync(obj);

                await Service.SaveChangesAsync();

                return new JsonResult(obj);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return NoContent();
            }
        }

        /// <summary>
        /// Gets the latest the version for a given platform and mobile app.
        /// </summary>
        /// <returns>The version.</returns>
        /// <param name="mobileApp">Mobile app.</param>
        /// <param name="platform">Platform.</param>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<MobileAppVersionViewModel>> LatestVersion(MobileApp mobileApp, MobilePlatform platform)
        {
            //TODO: Add parameter specifying device OS
            try
            {
                var latest = await Service.Latest(mobileApp, platform);

                var vm = Mapper.Map<MobileAppVersion, MobileAppVersionViewModel>(latest);

                return new JsonResult(vm);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return NoContent();
            }
        }

        /// <summary>
        /// Gets all latest mobile versions for every platform
        /// </summary>
        /// <returns>The version.</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<MobileAppVersionViewModel>> LatestVersions()
        {
            try
            {
                var latestVersions = Service.Latest();

                var result = Mapper.Map<IEnumerable<MobileAppVersion>, IEnumerable<MobileAppVersionViewModel>>(latestVersions);

                // Just to make sure the payload is ordered properly
                if (result?.Any() == true)
                {
                    result = result
                                .OrderBy(v => v.MobileApp)
                                .ThenBy(v => v.Platform);
                }

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
