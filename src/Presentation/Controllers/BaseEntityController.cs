using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;

namespace MGCap.Presentation.Controllers
{
    public class BaseEntityController<TEntity, TKey> : BaseController where TEntity : Entity<TKey>
    {
        public IBaseApplicationService<TEntity, TKey> AppService { get; set; }
        /// <summary>
        ///     Contains the base CRUD operations for a given entity
        /// </summary>
        /// <param name="employeeAppService"></param>
        /// <param name="mapper"></param>
        public BaseEntityController(
            IEmployeesApplicationService employeeAppService,
            IBaseApplicationService<TEntity, TKey> appService,
            IMapper mapper) : base(employeeAppService, mapper)
        {
            this.AppService = appService;
        }

        /// <summary>
        ///     Gets a <see cref="TEntity"/> obj in a 
        ///     specific output format
        /// </summary>
        /// <typeparam name="TDestination">Output type</typeparam>
        /// <param name="filter">Lambda that filters entity</param>
        /// <returns></returns>
        protected async Task<IActionResult> Get<TDestination>(Func<TEntity, bool> filter)
        {
            var obj = await this.AppService.SingleOrDefaultAsync(filter);
            if (obj == null)
            {
                return this.NotFound(this.ModelState);
            }

            var vm = this.Mapper.Map<TEntity, TDestination>(obj);
            return new JsonResult(vm);
        }

        protected async Task<IActionResult> Add<TSource, TDestination>(TSource vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var obj = this.Mapper.Map<TSource, TEntity>(vm);
            await this.AppService.AddAsync(obj);
            await this.AppService.SaveChangesAsync();
            var result = this.Mapper.Map<TEntity, TDestination>(obj);
            return new JsonResult(result);
        }

        protected async Task<IActionResult> Update<TSource>(TSource vm) where TSource : EntityViewModel<TKey>
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
            this.Mapper.Map(vm, obj);
            await this.AppService.UpdateAsync(obj);
            await this.AppService.SaveChangesAsync();
            var result = this.Mapper.Map<TEntity, TSource>(obj);
            return new JsonResult(result);
        }

        protected async Task<IActionResult> Delete(TKey id)
        {
            var obj = await this.AppService.SingleOrDefaultAsync(id);
            if (obj == null)
            {
                return this.NotFound(this.ModelState);
            }
            this.AppService.Remove(obj);
            await this.AppService.SaveChangesAsync();
            return this.Ok();
        }
    }
}
