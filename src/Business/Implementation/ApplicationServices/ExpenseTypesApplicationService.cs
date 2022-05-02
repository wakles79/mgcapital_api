// -----------------------------------------------------------------------
// <copyright file="ExpenseTypesApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.ExpenseSubcategory;
using MGCap.Domain.ViewModels.ExpenseType;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class ExpenseTypesApplicationService : BaseSessionApplicationService<ExpenseType, int>, IExpenseTypesApplicationService
    {
        public new IExpenseTypesRepository Repository => base.Repository as IExpenseTypesRepository;
        private readonly IExpenseSubcategoriesRepository ExpenseSubcategoriesRepository;

        public ExpenseTypesApplicationService(
            IExpenseTypesRepository repository,
            IExpenseSubcategoriesRepository expenseSubcategoriesRepository,
            IHttpContextAccessor httpContextAccessor
            ) : base(repository, httpContextAccessor)
        {
            this.ExpenseSubcategoriesRepository = expenseSubcategoriesRepository;
        }

        public Task<DataSource<ExpenseTypeGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int? isActive = -1)
        {
            return this.Repository.ReadAllDapperAsync(request, this.CompanyId, isActive);
        }

        public Task<DataSource<ExpenseTypeListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request)
        {
            return this.Repository.ReadAllCboDapperAsync(request, this.CompanyId);
        }

        public Task<ExpenseSubcategory> AddSubcategoryAsync(ExpenseSubcategory expenseSubcategory)
        {
            return this.ExpenseSubcategoriesRepository.AddAsync(expenseSubcategory);
        }

        public Task<ExpenseSubcategory> UpdateSubcategoryAsync(ExpenseSubcategory expenseSubcategory)
        {
            return this.ExpenseSubcategoriesRepository.UpdateAsync(expenseSubcategory);
        }

        public Task<ExpenseSubcategory> GetSubcategoryAsync(int id)
        {
            return this.ExpenseSubcategoriesRepository.SingleOrDefaultAsync(e => e.ID == id);
        }

        public Task<DataSource<ExpenseSubcategoryListBoxViewModel>> ReadAllSubcategoriesCboDapperAsync(DataSourceRequest request, int expenseTypeId, int? isEnabled)
        {
            return this.ExpenseSubcategoriesRepository.ReadAllCboDapperAsync(request, expenseTypeId, isEnabled);
        }

        public async Task<ExpenseType> UpdateStatusAsync(int id)
        {
            var expenseType = await this.Repository.SingleOrDefaultAsync(e => e.ID == id);
            expenseType.Status = !expenseType.Status;
            return await this.UpdateAsync(expenseType);
        }
    }
}
