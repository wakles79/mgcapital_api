using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Tag;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    public class TagsController : BaseEntityController<Tag, int>
    {
        private new ITagsApplicationService AppService => base.AppService as ITagsApplicationService;

        public TagsController
         (
             IEmployeesApplicationService employeeAppService,
             ITagsApplicationService appService,
             IMapper mapper
         ) : base(employeeAppService, appService, mapper)
        {

        }

        #region Tags
        [HttpGet]
        public async Task<IActionResult> ReadAll(DataSourceRequest request)
        {
            try
            {
                var result = await this.AppService.ReadAllDapperAsync(request);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReadAllCbo()
        {
            var result = await this.AppService.ReadAllCboDapperAsync();
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] TagCreateViewModel vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var tag = this.Mapper.Map<TagCreateViewModel, Tag>(vm);
            var tagObjet = await this.AppService.AddAsync(tag);
            await this.AppService.SaveChangesAsync();

            var result = this.Mapper.Map<Tag, TagCreateViewModel>(tagObjet);
            return new JsonResult(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await this.GetTagDetailAsync(id);

            if (result == null)
            {
                return this.NoContent();
            }

            return new JsonResult(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TagUpdateViewModel vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var tagObj = await this.AppService.SingleOrDefaultAsync(t => t.ID == vm.ID);
            if (tagObj == null)
            {
                return this.NoContent();
            }

            vm.Type = tagObj.Type;

            this.Mapper.Map(vm, tagObj);
            await this.AppService.UpdateAsync(tagObj);
            await this.AppService.SaveChangesAsync();

            var result = this.Mapper.Map<Tag, TagCreateViewModel>(tagObj);
            return new JsonResult(result);
        }

        [HttpDelete]
        public new async Task<IActionResult> Delete(int id)
        {
            var tagObj = await this.AppService.SingleOrDefaultAsync(t => t.ID == id);
            if (tagObj == null)
            {
                return BadRequest(this.ModelState);
            }

            try
            {
                this.AppService.Remove(tagObj.ID);
                await this.AppService.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        private async Task<TagBaseViewModel> GetTagDetailAsync(int id)
        {
            var tag = await this.AppService.SingleOrDefaultAsync(t => t.ID == id);
            if (tag == null)
            {
                return null;
            }

            var tagDetail = this.Mapper.Map<Tag, TagBaseViewModel>(tag);
            return tagDetail;
        }
        #endregion

        #region Ticket Tags
        [HttpGet]
        public async Task<IActionResult> ReadAllTicketTags(int ticketId)
        {
            var result = await this.AppService.ReadAllTicketTags(ticketId);
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddTicketTag([FromBody] TicketTagCreateViewModel vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var tag = this.Mapper.Map<TicketTagCreateViewModel, TicketTag>(vm);
            var ticketTagObj = await this.AppService.AddTicketTagAsync(tag);
            await this.AppService.SaveChangesAsync();

            var result = this.Mapper.Map<TicketTag, TicketTagCreateViewModel>(ticketTagObj);
            return new JsonResult(result);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveTicketTag([FromQuery] TicketTagDeleteViewModel vm)
        {
            try
            {
                await this.AppService.RemoveTicketTagAsync(vm.TicketTagId);
                await this.AppService.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        #endregion
    }
}
