
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers.MGClient
{
    public class CustomerFeedController : CustomerBaseController
    {
        private readonly ICustomerFeedService CustomerFeedService;

        public CustomerFeedController(ICustomerFeedService customerFeedService)
        {
            this.CustomerFeedService = customerFeedService;
        }

        #region Work Orders

        [HttpGet]
        public async Task<IActionResult> WorkOrdersReadAll (CustomerWODataSourceRequest dataRequest)
        {
            try
            {
                var result = await CustomerFeedService.WorkOrdersReadAllAsync(dataRequest);

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

        #endregion

        #region Clearing Reports

        [HttpGet]
        public async Task<IActionResult> CleaningReportsReadAll(CustomerCleaningReportDataSourceRequest dataRequest)
        {
            try
            {
                var result = await CustomerFeedService.CleaningReportsReadAllAsync(dataRequest);
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

        #endregion

        #region Buildings

        [HttpGet]
        public async Task<IActionResult> BuildingsReadAll(int contactId)
        {
            try
            {
                var result = await CustomerFeedService.BuildingsByContactIdAsync(contactId);

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

        #endregion
    }
}
