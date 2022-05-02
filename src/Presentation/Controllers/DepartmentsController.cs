using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Department;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MGCap.Presentation.Controllers
{
    public class DepartmentsController : BaseController
    {
        private readonly IDepartmentsApplicationService _departmentsApplicationService;
        private readonly IEmployeesApplicationService _employeesApplicationService;

        public DepartmentsController(
            IEmployeesApplicationService employeeAppService,
            IDepartmentsApplicationService departmentsApplicationService,
            IMapper mapper
            ) : base(employeeAppService, mapper)
        {
            _employeesApplicationService = employeeAppService;
            _departmentsApplicationService = departmentsApplicationService;
        }

        /// <summary>
        ///     Common method for retrieving all departments
        /// </summary>
        /// <returns>A list with all the Departments for the current Company</returns>
        // GET: api/departments/readall
        [HttpGet]
        public async Task<JsonResult> ReadAll(DataSourceRequest request)
        {
            var result = await this._departmentsApplicationService.ReadAllDapperAsync(request);
            return new JsonResult(result);
        }

        /// <summary>
        ///     Common method for retrieving all departments in a ListBoxViewModel
        /// </summary>
        /// <returns>A list with all the Departments for the current Company with format of ListBoxViewModel</returns>
        // GET: api/departments/readallcbo
        [HttpGet]
        public async Task<JsonResult> ReadAllCbo()
        {
            var departmentsObj = await _departmentsApplicationService.ReadAllAsync(ent => true);
            var departmentsVMs = departmentsObj.Select(d => new ListBoxViewModel { ID = d.ID, Name = d.Name }); ;
            return new JsonResult(departmentsVMs);
        }

        /// <summary>
        ///     Create a new departament
        /// </summary>
        /// <param id="id"></param>
        /// <returns>A response indicating the result of the request</returns>
        // POST: api/departments/add
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] DepartmentCreateViewModel departametVM)
        {
            if (this.ModelState.IsValid)
            {
                if (departametVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var departamentObj = this.Mapper.Map<DepartmentCreateViewModel, Department>(departametVM);
                await this._departmentsApplicationService.AddAsync(departamentObj);
                await this._departmentsApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }

        /// <summary>
        ///     Retrieving a departament by id
        /// </summary>
        /// <param id="id"></param>
        /// <returns>One departament</returns>
        // GET: api/departments/get/<id>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            return await GetDepartament(entity => entity.ID == id);
        }

        private async Task<IActionResult> GetDepartament(Func<Department, bool> func)
        {
            var departamentObj = await _departmentsApplicationService.SingleOrDefaultAsync(func);
            var departamentVM = this.Mapper.Map<Department, DepartmentDetailsViewModel>(departamentObj);
            return new JsonResult(departamentVM);
        }

        /// <summary>
        ///     Edit a departament by id
        /// </summary>
        /// <param id="id"></param>
        /// <returns>All fields that will be editables of the department</returns>
        //GET: api/departments/update/<id>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Update(int id)
        {
            var departmentObj = await _departmentsApplicationService.SingleOrDefaultAsync(ent => ent.ID == id);
            if (departmentObj == null)
            {
                return this.NotFound();
            }

            var departmentVMs = this.Mapper.Map<Department, DepartmentEditViewModel>(departmentObj);

            return new JsonResult(departmentVMs);
        }

        /// <summary>
        ///     Edit a departament by id
        /// </summary>
        /// <param id="id"></param>
        /// <returns>A response indicating the result of the request</returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DepartmentEditViewModel departmentVM)
        {
            if (this.ModelState.IsValid)
            {
                if (departmentVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var departmentObj = await _departmentsApplicationService.SingleOrDefaultAsync(ent => ent.ID == departmentVM.ID);
                if (departmentObj == null)
                {
                    return BadRequest(this.ModelState);
                }

                this.Mapper.Map(departmentVM, departmentObj);
                await this._departmentsApplicationService.UpdateAsync(departmentObj);

                await this._departmentsApplicationService.SaveChangesAsync();
                return Ok();
            }

            return BadRequest(this.ModelState);
        }

        /// <summary>
        ///     Delete a departament
        /// </summary>
        /// <param guid="Guid"></param>
        /// <returns>A response indicating the result of the request</returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var departmentObj = await _departmentsApplicationService.SingleOrDefaultAsync(ent => ent.ID == id);
            if (departmentObj == null)
            {
                return BadRequest(this.ModelState);
            }

            _departmentsApplicationService.Remove(departmentObj.ID);
            try
            {
                await this._departmentsApplicationService.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Do nothing
                return NoContent();
            }

            return Ok();

        }
    }
}
