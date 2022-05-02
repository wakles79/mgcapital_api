using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Proposal;
using MGCap.Domain.ViewModels.ProposalService;
using MGCap.Presentation.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    public class ProposalsController : BaseEntityController<Proposal, int>
    {
        public new IProposalsApplicationService AppService => base.AppService as IProposalsApplicationService;

        public ProposalsController(
            IEmployeesApplicationService employeeAppService,
            IProposalsApplicationService appService,
            IMapper mapper
            ) : base(employeeAppService, appService, mapper)
        {
        }

        #region Proposal
        [HttpGet]
        [PermissionsFilter("ReadProposals")]
        public async Task<ActionResult<DataSource<ProposalGridViewModel>>> ReadAll(DataSourceRequest request, int? status = -1)
        {
            var proposalVm = await AppService.ReadAllDapperAsync(request, status);
            return proposalVm;
        }

        [HttpPost]
        [PermissionsFilter("AddProposals")]
        public async Task<ActionResult> Add([FromBody] ProposalCreateViewModel proposalVm)
        {
            if (!this.ModelState.IsValid || proposalVm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var proposal = this.Mapper.Map<ProposalCreateViewModel, Proposal>(proposalVm);
            var proposalObject = await this.AppService.AddAsync(proposal);
            await this.AppService.SaveChangesAsync();

            var result = this.Mapper.Map<Proposal, ProposalCreateViewModel>(proposalObject);
            return new JsonResult(result);
        }

        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadProposals")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await this.GetProposalDetailAsync(id);

            if (result == null)
            {
                return this.NoContent();
            }

            return new JsonResult(result);
        }

        [HttpPut]
        [PermissionsFilter("UpdateProposals")]
        public async Task<IActionResult> Update([FromBody] ProposalUpdateViewModel proposalVM)
        {
            if (!this.ModelState.IsValid || proposalVM == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var proposalObject = await this.AppService.SingleOrDefaultAsync(p => p.ID == proposalVM.ID);
            if (proposalObject == null)
            {
                return this.NoContent();
            }

            this.Mapper.Map(proposalVM, proposalObject);
            await this.AppService.UpdateAsync(proposalObject);
            await this.AppService.SaveChangesAsync();
            var result = await this.GetProposalDetailAsync(proposalObject.ID);
            return new JsonResult(result);
        }

        [HttpDelete]
        [PermissionsFilter("DeleteProposals")]
        public async new Task<IActionResult> Delete(int id)
        {
            var reportObj = await this.AppService.SingleOrDefaultAsync(ent => ent.ID == id);
            if (reportObj == null)
            {
                return BadRequest(this.ModelState);
            }
            try
            {
                this.AppService.Remove(reportObj);
                await this.AppService.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, int status, string billToName, string billToEmail, int? billTo = -1)
        {
            var proposal = await this.GetProposalDetailAsync(id);
            if (proposal == null)
            {
                return this.NotFound();
            }

            try
            {
                await this.AppService.UpdateStatusAsync(id, status, billToName, billToEmail, billTo);
                await this.AppService.SaveChangesAsync();
            }
            catch (Exception)
            {

                return this.NoContent();
            }

            return this.Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetReportDetails(int id)
        {
            var proposalDetails = await this.AppService.GetProposalReportDetailsDapperAsync(id, null);
            return new JsonResult(proposalDetails);
        }

        [HttpPost]
        [PermissionsFilter("SendProposalByEmail")]
        public async Task<IActionResult> SendProposalReport([FromBody] ProposalReportSendEmailViewModel vm)
        {
            try
            {
                var proposalVm = await this.AppService.GetProposalReportDetailsDapperAsync(vm.ID, null);
                if (proposalVm == null)
                {
                    return BadRequest("Error finding report");
                }

                if (string.IsNullOrEmpty(proposalVm.CustomerEmail))
                {
                    return BadRequest("Customer doesn't have an email");
                }

                var additionalRecipents = vm.AdditionalRecipients;
                await this.AppService.SendProposalReport(proposalVm, additionalRecipents);
                return new JsonResult(proposalVm);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return BadRequest("Error Sending Emails");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> PublicGetReportDetails(Guid guid)
        {
            var proposalDetails = await this.AppService.GetProposalReportDetailsDapperAsync(null, guid);
            return new JsonResult(proposalDetails);
        }

        private async Task<ProposalDetailViewModel> GetProposalDetailAsync(int id)
        {
            var proposal = await this.AppService.SingleOrDefaultAsync(p => p.ID == id);
            if (proposal == null)
            {
                return null;
            }

            var proposalDetail = this.Mapper.Map<Proposal, ProposalDetailViewModel>(proposal);
            return proposalDetail;
        }
        #endregion

        #region Proposal Service
        [HttpGet]
        public async Task<ActionResult<DataSource<ProposalGridViewModel>>> ReadAllProposalServices(DataSourceRequest request, int proposalId)
        {
            var proposalServices = await this.AppService.ReadAllProposalServicesDapperAsync(request, proposalId);
            return new JsonResult(proposalServices);
        }

        [HttpPost]
        [PermissionsFilter("AddProposalService")]
        public async Task<ActionResult> AddProposalService([FromBody] ProposalServiceCreateViewModel proposalServiceVM)
        {
            if (!this.ModelState.IsValid || proposalServiceVM == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var obj = this.Mapper.Map<ProposalServiceCreateViewModel, ProposalService>(proposalServiceVM);
            var proposalServiceObject = await this.AppService.AddProposalServiceAsync(obj);
            await this.AppService.SaveChangesAsync();
            var result = this.Mapper.Map<ProposalService, ProposalServiceCreateViewModel>(obj);
            return new JsonResult(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetProposalService(int id)
        {
            var result = await this.GetProposalServiceDetailAsync(id);
            if (result == null)
            {
                return NoContent();
            }

            return new JsonResult(result);
        }

        [HttpPut]
        [PermissionsFilter("UpdateProposalService")]
        public async Task<ActionResult> UpdateProposalService([FromBody] ProposalServiceUpdateViewModel proposalServiceVM)
        {
            if (!this.ModelState.IsValid || proposalServiceVM == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var proposalServiceObject = await this.GetProposalServiceDetailAsync(proposalServiceVM.ID);
            if (proposalServiceObject == null)
            {
                return BadRequest(this.ModelState);
            }

            this.Mapper.Map(proposalServiceVM, proposalServiceObject);
            await this.AppService.UpdateProposalServiceAsync(proposalServiceObject);
            await this.AppService.SaveChangesAsync();

            var proposalServiceUpdated = await this.GetProposalServiceDetailAsync(proposalServiceObject.ID);
            var result = this.Mapper.Map<ProposalService, ProposalServiceDetailViewModel>(proposalServiceUpdated);
            return new JsonResult(result);
        }

        private async Task<ProposalService> GetProposalServiceDetailAsync(int id)
        {
            var proposalService = await this.AppService.GetProposalServiceByIdAsync(id);

            if (proposalService == null)
            {
                return null;
            }

            return proposalService;
        }
        #endregion
    }
}
