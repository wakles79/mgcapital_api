using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Contract;
using MGCap.Domain.ViewModels.ContractActivityLogNote;
using MGCap.Domain.ViewModels.ContractExpense;
using MGCap.Domain.ViewModels.ContractItem;
using MGCap.Domain.ViewModels.ContractNote;
using MGCap.Domain.ViewModels.ContractOfficeSpace;
using MGCap.Presentation.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MGCap.Presentation.Controllers
{
    public class ContractsController : BaseEntityController<Contract, int>
    {
        public new IContractsApplicationService AppService => base.AppService as IContractsApplicationService;
        public ContractsController(
            IEmployeesApplicationService employeeAppService,
            IContractsApplicationService appService,
            IMapper mapper) : base(employeeAppService, appService, mapper)
        {
        }

        #region Contract
        /// <summary>
        ///     Common method for retrieving all contracts in a ListBoxViewModel
        /// </summary>
        /// <returns>A list with all the contracts for the current Company with format of ListBoxViewModel</returns>
        [HttpGet]
        public async Task<JsonResult> ReadAllCbo(DataSourceRequest request, int? id)
        {
            var buildingsVM = await this.AppService.ReadAllCboDapperAsync(request, id);
            return new JsonResult(buildingsVM);
        }

        [HttpGet]
        public async Task<JsonResult> ReadAllByBuildingCbo(int buildingId, DateTime? date, int? contractId = null)
        {
            var result = await this.AppService.ReadAllCboByBuildingDapperAsync(buildingId, null, date, contractId);
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<JsonResult> ReadAllByCustomerCbo(int customerId, DateTime? date)
        {
            var result = await this.AppService.ReadAllCboByBuildingDapperAsync(null, customerId, date);
            return new JsonResult(result);
        }

        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadBudgets")]
        public async Task<IActionResult> Get(int id)
        {
            return await this.Get<ContractUpdateViewModel>(b => b.ID == id);
        }

        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadBudgets")]
        public async Task<IActionResult> Update(int id)
        {
            return await this.Get<ContractUpdateViewModel>(b => b.ID == id);
        }

        [HttpGet]
        [PermissionsFilter("ReadBudgets")]
        public async Task<JsonResult> ReadAll(DataSourceRequestBudget request, int? status)
        {
            var dataSource = await this.AppService.ReadAllDapperAsync(request, status);
            return new JsonResult(dataSource);
        }

        [HttpPost]
        [PermissionsFilter("AddBudgets")]
        public async Task<IActionResult> Add([FromBody] ContractCreateViewModel vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            // vm.ContractNumber = "499";
            vm.ContactSignerId = 85;
            vm.Area = 1999;
            vm.Status = ContractStatus.Pending;
            vm.DaysPerMonth = 21.5;
            vm.ProductionRate = 0;
            vm.ExpirationDate = null;
            vm.EditionCompleted = false;

            var obj = this.Mapper.Map<ContractCreateViewModel, Contract>(vm);

            await this.AppService.DisableBuildingContracts(vm.BuildingId);

            var ContractObj = await this.AppService.AddContractAsync(obj);

            await this.AppService.SaveChangesAsync();

            var result = await this.AppService.SingleOrDefaultAsync(c => c.ID == obj.ID);

            return new JsonResult(result);
        }

        [HttpPut]
        [PermissionsFilter("UpdateBudgets")]
        public async Task<IActionResult> Update([FromBody] ContractUpdateViewModel vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var obj = await this.AppService.SingleOrDefaultAsync(vm.ID);
            if (obj == null)
            {
                return this.BadRequest(this.ModelState);
            }

            bool editionCompleted = obj.EditionCompleted;

            // vm.ContractNumber = obj.ContractNumber;
            vm.ContactSignerId = obj.ContactSignerId;
            vm.Area = obj.Area;

            if (!obj.EditionCompleted)
            {
                vm.EditionCompleted = true;
            }

            this.Mapper.Map(vm, obj);
            await this.AppService.UpdateAsync(obj);
            await this.AppService.SaveChangesAsync();

            var result = this.Mapper.Map<Contract, ContractUpdateViewModel>(obj);

            if (!editionCompleted)
            {
                await this.AppService.AddDefaultItemsAndExpensesToContract(result.ID);
                // to save new contract items
                await this.AppService.SaveChangesAsync();
            }

            if (vm.UpdatePrepopulatedItems)
            {
                await this.AppService.UpdateDefaultItemsAndExpensesToContract(obj.ID);
            }

            // to refresh profit values in contract
            await this.AppService.SaveChangesAsync();
            
            await this.AppService.UpdateContractChildrenPeriodicityRate(result.ID, result.DaysPerMonth);
            // to refresh periodicity rate in contract children
            await this.AppService.SaveChangesAsync();

            await this.AppService.UpdateBudgetProfit(result.ID);
            // to refresh profit in contract
            await this.AppService.SaveChangesAsync();

            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetReportDetails(int id)
        {
            var contractsVM = await this.AppService.GetContractReportDetailsDapperAsync(id, null);
            return new JsonResult(contractsVM);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> PublicGetReportDetails(Guid guid)
        {
            var contractsVM = await this.AppService.GetContractReportDetailsDapperAsync(null, guid);
            return new JsonResult(contractsVM);
        }

        [HttpGet]
        public async Task<IActionResult> getReportDetailsBalances(DataSourceRequestInspection request, int id)
        {
            var contractsVM = await this.AppService.GetContractReportDetailsBalancesDapperAsync(id, null, request);
            return new JsonResult(contractsVM);
        }

        [HttpGet]
        public async Task<IActionResult> GetReportTrackingDetails(DataSourceRequest request, int? id, Guid? guid)
        {
            try
            {
                var result = await this.AppService.GetContractTrackingDetailsDapperAsync(request, id, guid);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<ActionResult> ValidateContractNumber(string contractNumber, int contractId = -1)
        {
            bool exist = await this.AppService.VerifyContractNumberExists(contractNumber, contractId);
            return new JsonResult(exist ? "Existing" : "Available");
        }

        [HttpDelete]
        [PermissionsFilter("DeleteBudgets")]
        public async Task<ActionResult> DeleteContract(int id)
        {
            var contract = await this.AppService.SingleOrDefaultAsync(c => c.ID == id);
            if (contract == null)
            {
                return NoContent();
            }

            try
            {
                await this.AppService.DeleteContractAsync(id);

                await this.AppService.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetContractSummary(int id)
        {
            var result = await this.AppService.GetContractSummaryAsync(id);
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveContractFromBuilding(int buildingId, int contractId = -1)
        {
            ContractSummaryViewModel result = null;
            var activeContracts = await this.AppService.ReadAllAsync(c => c.BuildingId == buildingId && c.Status == (int)ContractStatus.Active && c.ID != contractId);
            if (activeContracts.Count() > 0)
            {
                result = await this.AppService.GetContractSummaryAsync(activeContracts.First().ID);
            }
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetActivityLog(DataSourceRequest request, int id)
        {
            var result = await this.AppService.ReadAllActivityLog(request, id);
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetBudgetDetails(int id)
        {
            var budget = await this.AppService.GetBudgetDetail(id, null);
            return new JsonResult(budget);
        }
        // TODO: Define how change contract states
        /*[HttpDelete("api/contracts/delete/{id:int}")]
        public new async Task<IActionResult> Delete(int id)
        {
            var obj = await this.AppService.SingleOrDefaultAsync(id);
            if (obj == null)
            {
                return this.NotFound(this.ModelState);
            }
            this.AppService.Update(obj);
            await this.AppService.SaveChangesAsync();
            return this.Ok();
        }*/
        #endregion

        #region Contract Item
        [HttpPost]
        [PermissionsFilter("AddBudgetRevenue")]
        public async Task<IActionResult> AddContractItem([FromBody] ContractItemCreateViewModel contractItemVM)
        {
            if (this.ModelState.IsValid)
            {
                if (contractItemVM == null)
                {
                    return this.BadRequest(this.ModelState);
                }
                try
                {
                    var contractItemObj = this.Mapper.Map<ContractItemCreateViewModel, ContractItem>(contractItemVM);
                    await this.AppService.AddContractItemAsync(contractItemObj);
                    await this.AppService.SaveChangesAsync();

                    if (contractItemVM.UpdatePrepopulatedItems)
                    {
                        await this.AppService.UpdateDefaultItemsAndExpensesToContract(contractItemObj.ContractId);
                        await this.AppService.SaveChangesAsync();
                    }

                    await this.AppService.UpdateBudgetProfit(contractItemObj.ContractId);

                    // to refresh profit values in contract
                    await this.AppService.SaveChangesAsync();

                    var result = this.Mapper.Map<ContractItem, ContractItemCreateViewModel>(contractItemObj);
                    return new JsonResult(result);
                }
                catch (Exception ex)
                {
                    this.BadRequest(this.ModelState);
                }
            }
            return this.BadRequest(this.ModelState);
        }

        [HttpPost]
        [PermissionsFilter("UpdateBudgetRevenue")]
        public async Task<IActionResult> UpdateContractItem([FromBody] ContractItemUpdateViewModel contractItemVM)
        {
            if (this.ModelState.IsValid)
            {
                if (contractItemVM == null)
                {
                    return this.BadRequest(this.ModelState);
                }

                try
                {
                    var contractItemObj = this.Mapper.Map<ContractItemUpdateViewModel, ContractItem>(contractItemVM);
                    await this.AppService.UpdateContractItemAsync(contractItemObj);
                    await this.AppService.SaveChangesAsync();

                    if (contractItemVM.UpdatePrepopulatedItems)
                    {
                        await this.AppService.UpdateDefaultItemsAndExpensesToContract(contractItemObj.ContractId);
                        await this.AppService.SaveChangesAsync();
                    }

                    await this.AppService.UpdateBudgetProfit(contractItemObj.ContractId);

                    // to refresh profit values in contract
                    await this.AppService.SaveChangesAsync();

                    var result = this.Mapper.Map<ContractItem, ContractItemUpdateViewModel>(contractItemObj);
                    return new JsonResult(result);
                }
                catch (Exception ex)
                {
                    this.BadRequest(this.ModelState);
                }
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpGet("{contractId:int}")]
        public async Task<JsonResult> ReadAllContractItems(DataSourceRequest request, int contractId)
        {
            var contractItemsVM = await this.AppService.ReadAllContractItemsDapperAsync(request, contractId);
            return new JsonResult(contractItemsVM);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ContractItemUpdateViewModel>> GetContractItem(int id)
        {
            var contractItemObj = await this.AppService.GetContractItemByIdAsync(id);
            if (contractItemObj == null)
            {
                return this.NoContent();
            }

            var contractItemVM = this.Mapper.Map<ContractItem, ContractItemUpdateViewModel>(contractItemObj);
            return new JsonResult(contractItemVM);
        }

        [HttpDelete]
        [PermissionsFilter("DeleteBudgetRevenue")]
        public async Task<ActionResult> DeleteContractItem(int id, bool updatePrepopulated)
        {
            var item = await this.AppService.GetContractItemByIdAsync(id);
            if (item == null)
            {
                return this.NoContent();
            }

            try
            {
                await this.AppService.RemoveContractItemAsync(item);
                await this.AppService.SaveChangesAsync();

                if (updatePrepopulated)
                {
                    await this.AppService.UpdateDefaultItemsAndExpensesToContract(item.ContractId);
                    await this.AppService.SaveChangesAsync();
                }

                await this.AppService.UpdateBudgetProfit(item.ContractId);

                // to refresh profit values in contract
                await this.AppService.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Do Nothing
                return this.BadRequest("Error trying to delete");
            }

            return this.Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ReadAllRevenuesSquareFeet(int contractId)
        {
            var result = await this.AppService.ReadAllRevenuesSquareFeetAsync(contractId);
            return new JsonResult(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateContractRevenueOrder([FromBody] ContractItemUpdateOrderViewModel vm)
        {
            if (!this.ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await this.AppService.UpdateContractItemOrder(vm);
                await this.AppService.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Contract Expense
        [HttpPost]
        [PermissionsFilter("AddBudgetExpense")]
        public async Task<IActionResult> AddContractExpense([FromBody] ContractExpenseCreateViewModel contractExpenseVM)
        {
            if (this.ModelState.IsValid)
            {
                if (contractExpenseVM == null)
                {
                    return this.BadRequest(this.ModelState);
                }
                try
                {
                    var contractExpenseObj = this.Mapper.Map<ContractExpenseCreateViewModel, ContractExpense>(contractExpenseVM);
                    await this.AppService.AddContractExpenseAsync(contractExpenseObj);
                    await this.AppService.SaveChangesAsync();

                    await this.AppService.UpdateBudgetProfit(contractExpenseObj.ContractId);

                    // to refresh profit values in contract
                    await this.AppService.SaveChangesAsync();

                    var result = this.Mapper.Map<ContractExpense, ContractExpenseCreateViewModel>(contractExpenseObj);
                    return new JsonResult(result);
                }
                catch (Exception ex)
                {
                    this.BadRequest(this.ModelState);
                }
            }
            return this.BadRequest(this.ModelState);
        }

        [HttpPost]
        [PermissionsFilter("UpdateBudgetExpense")]
        public async Task<IActionResult> UpdateContractExpense([FromBody] ContractExpenseUpdateViewModel contractExpenseVM)
        {
            if (this.ModelState.IsValid)
            {
                if (contractExpenseVM == null)
                {
                    return this.BadRequest(this.ModelState);
                }

                try
                {
                    var contractExpenseObj = this.Mapper.Map<ContractExpenseUpdateViewModel, ContractExpense>(contractExpenseVM);
                    await this.AppService.UpdateContractExpenseAsync(contractExpenseObj);
                    await this.AppService.SaveChangesAsync();

                    await this.AppService.UpdateBudgetProfit(contractExpenseObj.ContractId);

                    // to refresh profit values in contract
                    await this.AppService.SaveChangesAsync();

                    var result = this.Mapper.Map<ContractExpense, ContractExpenseUpdateViewModel>(contractExpenseObj);
                    return new JsonResult(result);
                }
                catch (Exception ex)
                {
                    this.BadRequest(this.ModelState);
                }
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpGet("{contractId:int}")]
        public async Task<JsonResult> ReadAllContractExpenses(DataSourceRequest request, int contractId)
        {
            var contractExpensesVM = await this.AppService.ReadAllContractExpensesDapperAsync(request, contractId);
            return new JsonResult(contractExpensesVM);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ContractExpenseUpdateViewModel>> GetContractExpense(int id)
        {
            var contractExpenseObj = await this.AppService.GetContractExpenseByIdAsync(id);
            if (contractExpenseObj == null)
            {
                return this.NoContent();
            }

            var contractExpenseVM = this.Mapper.Map<ContractExpense, ContractExpenseUpdateViewModel>(contractExpenseObj);
            return new JsonResult(contractExpenseVM);
        }

        [HttpDelete]
        [PermissionsFilter("DeleteBudgetExpense")]
        public async Task<ActionResult> DeleteContractExpense(int id)
        {
            var expense = await this.AppService.GetContractExpenseByIdAsync(id);
            if (expense == null)
            {
                return this.NotFound();
            }

            try
            {
                await this.AppService.RemoveContractExpenseAsync(expense);
                await this.AppService.SaveChangesAsync();

                await this.AppService.UpdateBudgetProfit(expense.ContractId);

                // to refresh profit values in contract
                await this.AppService.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Do Nothing
                return this.BadRequest("Error trying to delete");
            }

            return this.Ok();
        }
        #endregion

        #region Office Spaces
        [HttpGet("{contractId:int}")]
        public async Task<ActionResult> ReadAllOfficeSpaces(DataSourceRequest request, int contractId)
        {
            var result = await this.AppService.ReadAllOfficeSpacesDapperAsync(request, contractId);
            return new JsonResult(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetOfficeSpaceById(int id)
        {
            var result = await this.AppService.GetOfficeSpaceByIdAsync(id);
            if (result == null)
            {
                return this.NoContent();
            }
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> AddOfficeSpace([FromBody] ContractOfficeSpaceCreateViewModel vm)
        {
            if (!ModelState.IsValid && vm == null)
            {
                return BadRequest(this.ModelState);
            }

            var officeSpace = this.Mapper.Map<ContractOfficeSpaceCreateViewModel, ContractOfficeSpace>(vm);
            var result = await this.AppService.AddOfficeSpaceAsync(officeSpace);

            await this.AppService.SaveChangesAsync();
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateOfficeSpace([FromBody] ContractOfficeSpaceUpdateViewModel vm)
        {
            if (!ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var officeSpaceObject = await this.AppService.GetOfficeSpaceByIdAsync(vm.ID);
            if (officeSpaceObject == null)
            {
                return this.NoContent();
            }

            this.Mapper.Map(vm, officeSpaceObject);
            await this.AppService.UpdateOfficeSpaceAsync(officeSpaceObject);
            await this.AppService.SaveChangesAsync();

            var result = await this.AppService.GetOfficeSpaceByIdAsync(vm.ID);
            return new JsonResult(result);
        }
        #endregion

        #region ImportCSV
        [HttpPost]
        public async Task<ActionResult> AddContractCSV([FromBody] ContractCreateViewModel revenueVm)
        {
            /*
            * 0 = Save
            * 1 = Customer not found
            * 2 = Building not found
            * 3 = Contract not found
            * 4 = 
            * 5 = Repetido/Actualizado 
            * 6 = Error
            */
            var Response = new ContractHttpCsvResponseViewModel
            {
                ErrorCode = 0,
                Message = "successful"
            };
            if (!this.ModelState.IsValid || revenueVm == null)
            {
                Response.ErrorCode = 6;
                Response.Message = "Invalid data";
                return new JsonResult(Response);
            }

            try
            {
                var result = await this.AppService.AddContractFromCsvAsync(revenueVm);
                await this.AppService.SaveChangesAsync();
                return new JsonResult(result);
            }
            catch (Exception)
            {
                Response.ErrorCode = 6;
                Response.Message = "Invalid data";
                return new JsonResult(6);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddContractItemCSV([FromBody] ContractItemCsvViewModel vm)
        {
            /*
            * 0 = Save
            * 1 = Customer not found
            * 2 = Building not found
            * 3 = Contract not found
            * 4 = 
            * 5 = Repetido/Actualizado 
            * 6 = Error
            */
            var Response = new ContractHttpCsvResponseViewModel
            {
                ErrorCode = 0,
                Message = "successful"
            };

            if (!this.ModelState.IsValid || vm == null)
            {
                Response.Message = "Invalid data";
                Response.ErrorCode = 6;
                return new JsonResult(Response);
            }

            try
            {
                var result = await this.AppService.AddContractItemFromCsvAsync(vm);
                await this.AppService.SaveChangesAsync();

                return new JsonResult(result);
            }
            catch (Exception)
            {
                Response.ErrorCode = 6;
                Response.Message = "Invalid data";
                return new JsonResult(Response);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddContractExpenseCsv([FromBody] ContractExpenseCsvViewModel vm)
        {
            /*
            * 0 = Save
            * 1 = Customer not found
            * 2 = Building not found
            * 3 = Contract not found
            * 4 = 
            * 5 = Repetido/Actualizado 
            * 6 = Error
             */

            var Response = new ContractHttpCsvResponseViewModel
            {
                ErrorCode = 0,
                Message = "successful"
            };

            if (!this.ModelState.IsValid || vm == null)
            {
                Response.ErrorCode = 6;
                Response.Message = "Invalid data";
                return new JsonResult(Response);
            }

            try
            {
                var result = await this.AppService.AddContractExpenseFromCsv(vm);
                await this.AppService.SaveChangesAsync();
                return new JsonResult(result);
            }
            catch (Exception)
            {
                Response.ErrorCode = 6;
                Response.Message = "Invalid data";
                return new JsonResult(Response);
            }
        }
        #endregion

        #region Export Csv
        [HttpGet]
        public async Task<FileResult> ExportContractToCsv(DataSourceRequestBudget request, int? status)
        {
            // HACK: To ensure all records are included within the daterange filter
            request.PageSize = int.MaxValue;
            request.PageNumber = 0;

            var vm = await this.AppService.ReadAllToCsvAsync(request, status);

            var csv = vm.ToCsv(true);

            byte[] bytes = Encoding.ASCII.GetBytes(csv);
            return File(bytes, "text/csv", "building-report.csv");
        }
        #endregion

        #region Export PDF Document
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBudgetPDFDocumentUrl(int id)
        {
            try
            {
                var result = await this.AppService.GetBudgetDocumentUrl(id);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        #endregion

        #region Notes
        [HttpPost]
        public async Task<IActionResult> AddNote([FromBody] ContractNoteCreateViewModel vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return BadRequest();
            }

            var obj = this.Mapper.Map<ContractNoteCreateViewModel, ContractNote>(vm);
            var note = await this.AppService.AddContractNoteAsync(obj);
            await this.AppService.SaveChangesAsync();

            return new JsonResult(note);
        }

        [HttpGet]
        public async Task<IActionResult> ReadAllNotes(int contractId)
        {
            try
            {
                var result = await this.AppService.ReadAllContractNotesAsync(contractId);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        #endregion

        #region Activity Log Notes
        [HttpGet]
        public async Task<IActionResult> ReadAllActivityLogNotes(int id)
        {
            try
            {
                var result = await this.AppService.ReadAllActivityLogNotesAsync(id);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetActivityLogNote(int id)
        {
            var result = await this.AppService.GetActivityLogNoteAsync(id);

            if (result == null)
            {
                return NoContent();
            }

            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddActivityLogNote([FromBody] ContractActivityLogNoteCreateViewModel vm)
        {
            if (vm == null || !this.ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var obj = this.Mapper.Map<ContractActivityLogNoteCreateViewModel, ContractActivityLogNote>(vm);

                var activityLogNote = await this.AppService.AddActivityLogNoteAsync(obj);

                await this.AppService.SaveChangesAsync();

                var result = this.Mapper.Map<ContractActivityLogNote, ContractActivityLogNoteUpdateViewModel>(activityLogNote);

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateActivityLogNote([FromBody] ContractActivityLogNoteUpdateViewModel vm)
        {
            if (vm == null || !this.ModelState.IsValid)
            {
                return BadRequest();
            }

            var note = await this.AppService.GetActivityLogNoteAsync(vm.ID);
            if (note == null)
                return NoContent();

            try
            {
                this.Mapper.Map(vm, note);

                var activityLogNote = await this.AppService.UpdateActivityLogNoteAsync(note);

                await this.AppService.SaveChangesAsync();

                var result = this.Mapper.Map<ContractActivityLogNote, ContractActivityLogNoteUpdateViewModel>(activityLogNote);

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveActivityLogNote(int id)
        {
            var note = await this.AppService.GetActivityLogNoteAsync(id);
            if (note == null)
                return NoContent();

            try
            {
                await this.AppService.RemoveActivityLogNoteAsync(note);

                await this.AppService.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}
