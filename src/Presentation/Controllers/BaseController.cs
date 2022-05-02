// -----------------------------------------------------------------------
// <copyright file="BaseController.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MGCap.Business.Abstract.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{

    /// <summary>
    ///     Contains the shared functionalities for the controllers
    /// </summary>
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class BaseController : Controller
    {
        public string AppBaseUrl => $"{this.Request.Scheme}://{this.Request.Host}/";
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        /// <param name="employeeAppService">To inject an instance of <see cref="IEmployeesApplicationService"/></param>
        /// <param name="mapper">To inject an instance of <see cref="IMapper"/></param>
        public BaseController(
            IEmployeesApplicationService employeeAppService,
            IMapper mapper)
        {
            this.EmployeeApplicationService = employeeAppService;
            this.Mapper = mapper;
        }

        public IMapper Mapper { get; }

        public IEmployeesApplicationService EmployeeApplicationService { get; }


    }

}
