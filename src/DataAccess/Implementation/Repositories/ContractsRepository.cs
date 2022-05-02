// -----------------------------------------------------------------------
// <copyright file="ContractsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Dapper;
using Microsoft.EntityFrameworkCore;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MGCap.Domain.ViewModels.Contract;
using MGCap.Domain.ViewModels.ContractItem;
using MGCap.Domain.ViewModels.ContractExpense;
using MGCap.Domain.ViewModels.Revenue;
using MGCap.Domain.ViewModels.Expense;

using MGCap.Domain.ViewModels.ContractOfficeSpace;
using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.ContractNote;
using MGCap.Domain.ViewModels.ContractActivityLog;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the functionalities
    ///     for handling operations on the <see cref="Contract"/>
    /// </summary>
    public class ContractsRepository : BaseRepository<Contract, int>, IContractsRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;
        /// <summary>
        ///     Initializes a new instance of the <see cref="ContractsRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
        public ContractsRepository(
            MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository
            )
            : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        //private int NextNumber(int companyId)
        //{
        //    var maxNumber = 0;
        //    if (this.Entities != null && this.Entities.Count() > 0)
        //    {
        //        maxNumber = this.Entities
        //                            ?.Where(ent => ent.CompanyId == companyId)
        //                            ?.DefaultIfEmpty()
        //                            ?.Max(ent => int.Parse(ent.ContractNumber)) ?? 0;
        //    }
        //    return maxNumber + 1;
        //}

        public override Task<Contract> AddAsync(Contract obj)
        {
            // this value is added by the user
            // obj.ContractNumber = this.NextNumber(obj.CompanyId).ToString();

            return base.AddAsync(obj);
        }

        public async Task<DataSource<ContractGridViewModel>> ReadAllDapperAsync(DataSourceRequestBudget request, int companyId, int? status)
        {
            var result = new DataSource<ContractGridViewModel>
            {
                Payload = new List<ContractGridViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) && string.IsNullOrEmpty(request.SortField) ? " ID DESC " : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string where = string.Empty;
            if (request.UpdatedDateFrom.HasValue)
            {
                where += " AND C.UpdatedDate >= @updatedDateFrom";
            }

            if (request.UpdatedDateTo.HasValue)
            {
                where += " AND C.UpdatedDate <= @updatedDateTo";
            }

            if (request.BuildingId.HasValue)
            {
                where += " AND C.BuildingId = @buildingId ";
            }

            if (request.CustomerId.HasValue)
            {
                where += " AND C.CustomerId = @customerId ";
            }

            if (request.CreatedDate.HasValue)
            {
                where += " AND CAST(C.CreatedDate AS date) = CAST(@createdDate AS date) ";
            }

            string query = $@"
                    -- payload query
                SELECT *, [Count] = COUNT(*) OVER()
                FROM (
                    SELECT C.ID,                           
	                       C.ContractNumber,
	                       B.Name as BuildingName,
                           (CASE WHEN B.Code = '' THEN CAST(B.ID AS VARCHAR) ELSE B.Code END) AS BuildingCode,
                           CC.ID as CustomerId,
	                       CC.Name as CustomerFullName,
                           (CASE WHEN CC.Code = '' THEN CAST(CC.ID AS VARCHAR) ELSE CC.Code END) AS CustomerCode,
                           C.ExpirationDate,
                           C.UpdatedDate,
	                       C.Status,
                           (SELECT SUM(CI.SquareFeet) FROM ContractItems CI WHERE CI.ContractId = C.ID AND CI.RateType = {(int)ServiceRateType.SquareFeet}) AS OccupiedSquareFeets,
                           C.[DailyProfit],
                           C.[MonthlyProfit],
                           C.[YearlyProfit],
                           C.[DailyProfitRatio],
                           C.[MonthlyProfitRatio],
                           C.[YearlyProfitRatio],
		                    (SELECT SUM(CI.[MonthlyRate]) FROM [dbo].[ContractItems] AS CI WHERE CI.[ContractId]=C.ID) AS TotalMonthlyRevenue,
		                    (SELECT SUM(CE.[MonthlyRate]) FROM [dbo].[ContractExpenses] AS CE WHERE CE.[ContractId]=C.ID) AS TotalMonthlyExpense,
                            (SELECT SUM(CE.[MonthlyRate]) FROM [dbo].[ContractExpenses] AS CE WHERE CE.[ContractId]=C.ID  AND CE.ExpenseCategory = {(int)ExpenseCategory.Labor}) AS TotalMonthlyLaborExpense
                     FROM [dbo].[Contracts] as C 
	                 INNER JOIN [dbo].[Buildings] as B ON C.BuildingId = B.ID
	                 INNER JOIN [dbo].[Customers] as CC ON C.CustomerId = CC.ID

                    WHERE C.CompanyId = @companyId 
                        AND ((@isActive IS NOT NULL AND C.[Status] = @isActive) OR (@isActive IS NULL))
                        AND ((@filter IS NOT NULL AND CONCAT(C.[ContractNumber], B.[Name], B.[Code], CC.[Name], CC.[Code]) LIKE '%'+@filter+'%') OR (@filter IS NULL)) {where}
                ) payload 

                    ORDER BY {orders}
                    OFFSET @pageSize * @pageNumber ROWS
                    FETCH NEXT @pageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);
            pars.Add("@isActive", status == -1 ? null : status);

            pars.Add("@updatedDateFrom", request.UpdatedDateFrom);
            pars.Add("@updatedDateTo", request.UpdatedDateTo);
            pars.Add("@buildingId", request.BuildingId);
            pars.Add("@customerId", request.CustomerId);
            pars.Add("@createdDate", request.CreatedDate);

            var payload = await _baseDapperRepository.QueryAsync<ContractGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            if (result.Count > 0)
            {
                payload.FirstOrDefault().TotalProfitRatio = payload.FirstOrDefault().TotalProfitRatio / result.Count;
            }
            result.Payload = payload;

            return result;
        }

        public async Task<DataSource<ContractExportCsvViewModel>> ReadAllCsvDapperAsync(DataSourceRequestBudget request, int companyId, int? status)
        {
            var result = new DataSource<ContractExportCsvViewModel>
            {
                Payload = new List<ContractExportCsvViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) && string.IsNullOrEmpty(request.SortField) ? " ID DESC " : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string where = string.Empty;
            if (request.UpdatedDateFrom.HasValue)
            {
                where += " AND C.UpdatedDate >= @updatedDateFrom";
            }

            if (request.UpdatedDateTo.HasValue)
            {
                where += " AND C.UpdatedDate <= @updatedDateTo";
            }

            if (request.BuildingId.HasValue)
            {
                where += " AND C.BuildingId = @buildingId ";
            }

            if (request.CreatedDate.HasValue)
            {
                where += " AND CAST(C.CreatedDate AS date) = CAST(@createdDate AS date) ";
            }

            string query = $@"
                    -- payload query
                SELECT *, [Count] = COUNT(*) OVER() 
                FROM (
                    SELECT
                            C.ID,
                            (B.Name) AS Building,
                            (
                                SELECT TOP 1 CU.[Name]
                                FROM [dbo].[Contracts] AS CO
                                    INNER JOIN [dbo].[Customers] AS CU ON CU.ID = CO.CustomerId
                                WHERE CO.[BuildingId] = B.ID AND CO.[Status] = 1
                                ORDER BY CO.ID DESC
                            ) AS MGMTCompany,
                            (SELECT SUM(CI.SquareFeet) FROM ContractItems CI WHERE CI.ContractId = C.ID AND CI.RateType = {(int)ServiceRateType.SquareFeet}) AS OccupiedSqFt,
                            (SELECT SUM(CI.[MonthlyRate]) FROM [dbo].[ContractItems] CI WHERE CI.[ContractId] = C.[ID]) AS MonthlyAmount,
                            --('0') AS BillableRequests,
                            (SELECT SUM(CE.MonthlyRate) FROM [dbo].[ContractExpenses] AS CE WHERE CE.[ContractId] = C.[ID] AND CE.[ExpenseSubcategoryName] = 'Supervisor') AS Supervisor,
                            (SELECT SUM(CE.MonthlyRate) FROM [dbo].[ContractExpenses] AS CE WHERE CE.[ContractId] = C.[ID] AND CE.[ExpenseSubcategoryName] = 'Dayporter') AS Dayporter,
                            (SELECT SUM(CE.MonthlyRate) FROM [dbo].[ContractExpenses] AS CE WHERE CE.[ContractId] = C.[ID] AND CE.[ExpenseSubcategoryName] = 'Worker') AS Worker,
                            (SELECT SUM(CE.MonthlyRate) FROM [dbo].[ContractExpenses] AS CE WHERE CE.[ContractId] = C.[ID] AND CE.[ExpenseSubcategoryName] = 'Admin/Operations') AS OperationsAdmin,
                            (SELECT SUM(CE.MonthlyRate) FROM [dbo].[ContractExpenses] AS CE WHERE CE.[ContractId] = C.[ID] AND CE.[ExpenseSubcategoryName] = 'Van Crew') AS Van,
                            (
                                SELECT 
                                SUM(CE.MonthlyRate) * 0.14
                                FROM [dbo].[ContractExpenses] AS CE
                                WHERE CE.ExpenseCategory = 0 AND CE.ContractId = C.ID   
                            ) AS EmployeeTax,
                            ('0') AS Overhead,
                            (
                                SELECT SUM(EX.[Value])
                                    FROM [dbo].[ContractExpenses] AS EX
                                WHERE EX.ContractId = C.ID AND EX.ExpenseCategory = {(int)ExpenseCategory.Supplies}
                            ) AS TotalSupplies,
                            (CS.[FederalInsuranceContributionsAct]) AS Insurance,
                            (SELECT SUM(CE.MonthlyRate) FROM [dbo].[ContractExpenses] AS CE WHERE CE.[ContractId] = C.[ID] AND CE.[ExpenseSubcategoryName] = 'Phone') AS Phone,
                            (SELECT SUM(CE.MonthlyRate) FROM [dbo].[ContractExpenses] AS CE WHERE CE.[ContractId] = C.[ID] AND CE.[ExpenseSubcategoryName] = 'Uniform') AS Uniform,
                            (SELECT SUM(CE.[MonthlyRate]) FROM [dbo].[ContractExpenses] CE WHERE CE.[ContractId] = C.[ID]) AS TotalExpense,
                            (C.MonthlyProfit) AS GrossProfit,
                            (C.MonthlyProfitRatio) AS Profit
                     FROM [dbo].[Contracts] as C 
                         INNER JOIN [dbo].[Buildings] as B ON C.BuildingId = B.ID
                         INNER JOIN [dbo].[Customers] as CC ON C.CustomerId = CC.ID
                         LEFT OUTER JOIN [dbo].[CompanySettings] CS ON CS.CompanyId = C.CompanyId
                    WHERE C.CompanyId = @companyId 
                        AND ((@isActive IS NOT NULL AND C.[Status] = @isActive) OR (@isActive IS NULL))
                        AND ((@filter IS NOT NULL AND CONCAT(C.[ContractNumber], B.[Name], B.[Code], CC.[Name], CC.[Code]) LIKE '%'+@filter+'%') OR (@filter IS NULL)) {where}
                ) payload 

                    ORDER BY {orders}
                    OFFSET @pageSize * @pageNumber ROWS
                    FETCH NEXT @pageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);
            pars.Add("@isActive", status == -1 ? null : status);

            pars.Add("@updatedDateFrom", request.UpdatedDateFrom);
            pars.Add("@updatedDateTo", request.UpdatedDateTo);
            pars.Add("@buildingId", request.BuildingId);
            pars.Add("@createdDate", request.CreatedDate);

            var payload = await _baseDapperRepository.QueryAsync<ContractExportCsvViewModel>(query, pars);
            result.Count = 0;
            result.Payload = payload;

            return result;
        }

        public async Task<DataSource<ListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int companyId, int? id = null)
        {
            var result = new DataSource<ListBoxViewModel>
            {
                Payload = new List<ListBoxViewModel>(),
                Count = 0
            };
            //  select @filter = '', @companyId = 5, @pageSize = 50, @pageNumber = 0;
            string query = $@"                    
                    declare @index int;
                    declare @maxIndex int;
                    declare @total int;

                    --select @id = 940;

                    IF ISNULL(@id, 0) <= 0 OR ISNULL(@filter, '') <> ''
                    BEGIN
                        select @index =  @pageNumber;
                    END
                    ELSE
                    BEGIN
                    SELECT @index = [Index] - 1 FROM ( 
                        SELECT 
		                    C.ID as ID, 
		                    C.CompanyId as CompanyId,
                            ROW_NUMBER() OVER (PARTITION BY CompanyId Order BY C.ContractNumber, C.ID ) as [Index]
                        FROM [dbo].[Contracts] as C
                        ) payload
                    WHERE ID = @id
                    END

                    SELECT @total = COUNT(*) FROM [dbo].[Contracts] WHERE [dbo].[Contracts].[CompanyId]= @companyId;

                    --max(0, @total-@pageSize)
                    SELECT @maxIndex = IIF(@pageSize > @total, 0, @total - @pageSize);

                    --safety check
                    SELECT @index = ISNULL(@index, 0);

                    --min(@index, @maxIndex)
                    SELECT @index = IIF(@index > @maxIndex, @maxIndex, @index)

                    SELECT 
	                    CC.ID as ID,
	                    CONCAT(CC.ContractNumber,' ', B.Name) as [Name]
                    FROM [dbo].[Contracts] as CC INNER JOIN [dbo].[Buildings] as B ON CC.BuildingId = B.ID
                    WHERE CC.CompanyId= @companyId AND
	                    ISNULL(CONCAT(CC.ContractNumber,' ', B.Name), '') 
		                    LIKE '%' + ISNULL(@filter, '') + '%'
                    ORDER BY Name, ID

                    OFFSET @index ROWS
                    FETCH NEXT @pageSize ROWS ONLY
                    ";
            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@id", id);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);

            var payload = await _baseDapperRepository.QueryAsync<ListBoxViewModel>(query, pars);
            result.Count = payload?.Count() ?? 0;
            result.Payload = payload;

            return result;

        }

        public async Task<Contract> SingleOrDefaultContractByBuildingAsync(int buildingId)
        {
            return await this.Entities.FirstOrDefaultAsync(c => c.BuildingId == buildingId && c.Status == 1);
        }

        public async Task<ContractReportDetailViewModel> GetContractReportDetailsDapperAsync(int? contractId, Guid? guid)
        {
            var result = new ContractReportDetailViewModel();
            var pars = new DynamicParameters();
            string whereQuery = "";

            if (guid.HasValue)
            {
                whereQuery = " C.Guid = @ContractGuid";
                pars.Add("@ContractGuid", guid);
            }
            else
            {
                whereQuery = " C.ID = @ContractId";
                pars.Add("@ContractId", contractId);
            }

            string contractQuery = $@"
                                    SELECT 
                                        C.ID,
                                        C.Guid,
                                        C.ContractNumber,
                                        C.Area,
                                        C.NumberOfPeople,
                                        C.BuildingId,
                                        C.CustomerId,
                                        C.ContactSignerId,
                                        C.Status,
                                        C.Description,
                                        C.DaysPerMonth,
                                        C.NumberOfRestrooms,
                                        C.ExpirationDate,
                                        C.UnoccupiedSquareFeets,
                                        C.ProductionRate,
                                        CS.Name AS CustomerName,
                                        B.Name AS BuildingName
                                    FROM [dbo].[Contracts] AS C
                                        INNER JOIN Customers CS ON CS.ID = C.CustomerId
                                        INNER JOIN Buildings B ON B.ID = C.BuildingId
                                    WHERE { whereQuery }
                ";

            var contractData = await this._baseDapperRepository.QueryAsync<ContractReportDetailViewModel>(contractQuery, pars);
            result = contractData.FirstOrDefault();

            if (guid.HasValue)
            {
                pars.Add("@ContractId", result.ID);
            }

            string contractItemsQuery = @"
                                        SELECT 
                                            CI.ID,
                                            CI.Quantity,
                                            CI.Description,
                                            CI.OfficeServiceTypeId,
                                            CI.OfficeServiceTypeName,
                                            CI.Rate,
                                            CI.RateType,
                                            CI.RatePeriodicity,
                                            CI.SquareFeet,
                                            CI.Amount,
                                            CI.Hours,
                                            CI.Rooms,
                                            CI.DefaultType
                                        FROM [dbo].[ContractItems] AS CI
                                        WHERE CI.ContractId = @ContractId";

            var contractItemsSource = await this._baseDapperRepository.QueryAsync<ContractItemGridViewModel>(contractItemsQuery, pars);
            if (contractItemsSource.Any())
                result.ContractItems = contractItemsSource;

            string contractExpensesQuery = @"
                                            SELECT  
                                            CE.ID,
                                            CE.Quantity,
                                            CE.Description,
                                            CE.ExpenseTypeName,
                                            CE.ExpenseCategory,
                                            CE.ExpenseSubcategoryId,
                                            CE.ExpenseSubcategoryName,
                                            CE.Rate,
                                            CE.RateType,
                                            CE.RatePeriodicity,
                                            CE.Value,
                                            CE.OverheadPercent,
                                            CASE WHEN ES.ID IS NOT NULL THEN ES.ExpenseTypeId ELSE 0 END AS ExpenseTypeId,
                                            CE.DefaultType
                                            FROM [dbo].[ContractExpenses] AS CE LEFT JOIN [dbo].[ExpenseSubcategories] AS ES ON ES.ID=CE.ExpenseSubcategoryId
                                            WHERE CE.ContractId = @ContractId";

            var contractExpensesSource = await this._baseDapperRepository.QueryAsync<ContractExpenseGridViewModel>(contractExpensesQuery, pars);
            if (contractExpensesSource.Any())
                result.ContractExpenses = contractExpensesSource;

            string contractOfficeSpacesQuery = $@"
                                            SELECT 
	                                            CO.[ID],
	                                            CO.[ContractId],
	                                            CO.[OfficeTypeId],
	                                            CO.[SquareFeet],
                                                OT.[Name] AS OfficeTypeName
                                            FROM [dbo].[ContractOfficeSpaces] AS CO
                                                LEFT JOIN OfficeServiceTypes OT ON OT.ID = CO.OfficeTypeId
                                            WHERE CO.ContractId = @ContractId";
            var contractOfficeSpaces = await this._baseDapperRepository.QueryAsync<ContractOfficeSpaceGridViewModel>(contractOfficeSpacesQuery, pars);
            if (contractOfficeSpaces.Any())
                result.OfficeSpaces = contractOfficeSpaces;

            return result;
        }

        public async Task<DataSource<ContractListBoxViewModel>> ReadAllCboByBuildingDapperAsync(int companyId, int? buildingId, int? customerId, DateTime? date, int? contractId = null)
        {
            var result = new DataSource<ContractListBoxViewModel>
            {
                Payload = new List<ContractListBoxViewModel>(),
                Count = 0
            };

            // string where = " AND CC.ExpirationDate >= GETUTCDATE()";
            string where = "";
            string selectProperties = "CONCAT(CC.ContractNumber,' - ', C.Name) as [Name]";

            if (date.HasValue)
            {
                where = " AND CC.ExpirationDate >= @date";
            }

            if (buildingId.HasValue)
            {
                where += " AND CC.BuildingId = @buildingId";
            }

            if (customerId.HasValue)
            {
                selectProperties = "CONCAT(CC.ContractNumber,' - ', B.Name) as [Name]";
                where += " AND CC.CustomerId = @customerId";
            }

            if (contractId.HasValue)
            {
                where += " or CC.ID = @contractId";
            }


            string query = $@"
                    SELECT 
                        CC.ID as ID,
                        B.ID AS BuildingId,
                        C.ID AS CustomerId,
                        {selectProperties}
                    FROM [dbo].[Contracts] AS CC
                        INNER JOIN [dbo].[Buildings] as B ON CC.BuildingId = B.ID
                        INNER JOIN [dbo].[Customers] AS C ON CC.CustomerId = C.ID
                    WHERE CC.CompanyId = @companyId {where} 
                    ORDER BY CC.ContractNumber";

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@buildingId", buildingId);
            pars.Add("@customerId", customerId);
            pars.Add("@contractId", contractId);
            pars.Add("@date", date);

            var payload = await _baseDapperRepository.QueryAsync<ContractListBoxViewModel>(query, pars);
            result.Count = payload?.Count() ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<ContractReportDetailViewModel> GetContractReportDetailsBalancesDapperAsync(int? contractId, Guid? guid, DataSourceRequest request)
        {
            string whereStr = "";

            if (request.DateFrom.HasValue)
            {
                whereStr += $" AND CAST(RE.Date  AS DATE) >= @dateFrom ";
            }

            if (request.DateTo.HasValue)
            {
                whereStr += $" AND CAST(RE.Date  AS DATE) <= @dateTo ";
            }

            var result = new ContractReportDetailViewModel();
            var pars = new DynamicParameters();
            string whereQuery = "";

            if (guid.HasValue)
            {
                whereQuery = " C.Guid = @ContractGuid";
                pars.Add("@ContractGuid", guid);
            }
            else
            {
                whereQuery = " C.ID = @ContractId";
                pars.Add("@ContractId", contractId);
            }

            string contractQuery = $@"
                                    SELECT 
                                        C.ID,
                                        C.Guid,
                                        C.ContractNumber,
                                        C.Area,
                                        C.NumberOfPeople,
                                        C.BuildingId,
                                        C.CustomerId,
                                        C.ContactSignerId,
                                        C.Status,
                                        C.Description,
                                        C.DaysPerMonth,
                                        C.NumberOfRestrooms,
                                        C.ExpirationDate,
                                        CS.Name AS CustomerName,
                                        B.Name AS BuildingName
                                    FROM [dbo].[Contracts] AS C
                                        INNER JOIN Customers CS ON CS.ID = C.CustomerId
                                        INNER JOIN Buildings B ON B.ID = C.BuildingId
                                    WHERE { whereQuery }
                ";

            var contractData = await this._baseDapperRepository.QueryAsync<ContractReportDetailViewModel>(contractQuery, pars);
            result = contractData.FirstOrDefault();

            if (guid.HasValue)
            {
                pars.Add("@ContractId", result.ID);
            }

            pars.Add("@dateFrom", request.DateFrom);
            pars.Add("@dateTo", request.DateTo);

            string contractRevenuesQuery = $@"
                                        SELECT 
                                             RE.ID,
                                             RE.Tax,                                             
                                             RE.Date,
                                             RE.SubTotal,
                                             RE.Total,
                                             RE.Reference,
                                             RE.Description,
											 RE.BuildingId,
											 RE.ContractId,
											 RE.CustomerId
                                        FROM [dbo].Revenues AS RE
                                        WHERE RE.ContractId = @ContractId  {whereStr}";

            var contractRevenuesSource = await this._baseDapperRepository.QueryAsync<RevenueGridViewModel>(contractRevenuesQuery, pars);
            if (contractRevenuesSource.Any())
                result.Revenues = contractRevenuesSource;

            string contractExpensesQuery = $@"
                                     SELECT 
                                         RE.ID,
                                         RE.[Type],
                                         RE.[Amount],
                                         RE.[Date],
                                         RE.[Vendor],
                                         RE.[Description],
                                         RE.[Reference]
                                     FROM [dbo].[Expenses] AS RE
                                     WHERE RE.ContractId =  @ContractId   {whereStr}";

            var contractExpensesSource = await this._baseDapperRepository.QueryAsync<ExpenseGridViewModel>(contractExpensesQuery, pars);
            if (contractExpensesSource.Any())
                result.Expenses = contractExpensesSource;

            return result;
        }

        public async Task<ContractTrackingDetailViewModel> GetContractTrackingDetailsDapperAsync(DataSourceRequest request, int? id, Guid? guid)
        {
            ContractTrackingDetailViewModel result = new ContractTrackingDetailViewModel();

            var pars = new DynamicParameters();
            string whereQuery = "";

            if (guid.HasValue)
            {
                whereQuery = " C.[Guid] = @ContractGuid";
                pars.Add("@ContractGuid", guid);
            }
            else
            {
                whereQuery = " C.[ID] = @ContractId";
                pars.Add("@ContractId", id);
            }

            string revenuesWhere = string.Empty;
            string expensesWhere = string.Empty;
            if (request.DateFrom.HasValue)
            {
                revenuesWhere += " AND CAST(R.[Date] AS DATE) >= @dateFrom";
                expensesWhere += " AND CAST(E.[Date] AS DATE) >= @dateFrom";
            }

            if (request.DateTo.HasValue)
            {
                revenuesWhere += " AND CAST(R.[Date] AS DATE) <= @dateTo";
                expensesWhere += " AND CAST(E.[Date] AS DATE) <= @dateTo";
            }

            string queryContract = $@"
                SELECT
	                C.[ID],
	                C.[ContractNumber],
	                C.[Area],
	                C.[NumberOfPeople],
	                C.[BuildingId],
	                C.[CustomerId],
	                C.[ContactSignerId],
	                C.[Status],
	                C.[Description],
	                C.[DaysPerMonth],
	                C.[NumberOfRestrooms],
	                C.[FrequencyPerYear],
	                C.[ExpirationDate],
	                C.[ProductionRate],
	                C.[UnoccupiedSquareFeets],	
	                B.[Name] AS BuildingName,
	                CS.[Name] AS CustomerName
                FROM [dbo].[Contracts] AS C
	                LEFT  JOIN [dbo].[Buildings] B ON B.ID = C.BuildingId
	                LEFT JOIN [dbo].[Customers] CS ON CS.ID = C.CustomerId
                WHERE {whereQuery} ";

            var contractData = await this._baseDapperRepository.QueryAsync<ContractTrackingDetailViewModel>(queryContract, pars);
            result = contractData.FirstOrDefault();

            if (guid.HasValue)
                pars.Add("@ContractId", result.ID);

            pars.Add("dateFrom", request.DateFrom);
            pars.Add("dateTo", request.DateTo);

            string queryRevenues = $@"
                SELECT 
	                R.[ID],
	                R.[ContractId],
	                B.[ID] AS [BuildingId],
	                B.[Name] AS BuildingName,
	                C.[ID] AS [CustomerId],
	                C.[Name] AS CustomerName,
	                R.[Date],
	                R.[SubTotal],
	                R.[Tax],
	                R.[Total],
	                R.[Description],
	                R.[Reference],
                    R.[TransactionNumber],
                    CS.[ContractNumber]
                FROM [dbo].[Revenues] AS R
	                LEFT JOIN [dbo].[Contracts] CS ON CS.ID = R.ContractId
	                LEFT JOIN [dbo].[Buildings] B ON B.ID = CS.BuildingId
	                LEFT JOIN [dbo].[Customers] C ON C.ID = CS.CustomerId
                WHERE R.[ContractId] = @ContractId {revenuesWhere}
                ORDER BY R.[Date] ASC";

            var contractRevenuesSource = await this._baseDapperRepository.QueryAsync<RevenueGridViewModel>(queryRevenues, pars);
            if (contractRevenuesSource.Any())
                result.Revenues = contractRevenuesSource;

            string queryExpenses = $@"
                SELECT
	                E.[ID],
	                E.[ContractId],
	                B.[ID] AS [BuildingId],
	                B.[Name] AS BuildingName,
	                C.[ID] AS [CustomerId],
	                C.[Name] AS CustomerName,
	                E.[Date],
	                E.[Vendor],
	                E.[Type],
	                E.[Amount],
	                E.[Description],
                    E.[Reference],
                    E.[TransactionNumber],
                    CS.[ContractNumber]
                FROM [dbo].[Expenses] E
	                LEFT JOIN [dbo].[Contracts] CS ON CS.ID = E.ContractId
	                LEFT JOIN [dbo].[Buildings] B ON B.ID = CS.BuildingId
	                LEFT JOIN [dbo].[Customers] C ON C.ID = CS.CustomerId
                WHERE E.[ContractId] = @ContractId {expensesWhere}
                ORDER BY E.[Date] ASC";

            var contractExpensesSource = await this._baseDapperRepository.QueryAsync<ExpenseGridViewModel>(queryExpenses, pars);
            if (contractExpensesSource.Any())
                result.Expenses = contractExpensesSource;

            return result;
        }

        public async Task<ContractSummaryViewModel> GetContractSummaryDapperAsync(int id)
        {
            ContractSummaryViewModel contractSummary = new ContractSummaryViewModel();

            string query = $@"
                -- Get Contract Summary Query
                SELECT
	                C.[ID],
	                C.[ContractNumber],
	                C.[Description],
	                C.[Status],
	                (SELECT COUNT(CI.ID) FROM [dbo].[ContractItems] AS CI WHERE CI.ContractId = C.ID) AS TotalEstimatedRevenue,
	                (SELECT COUNT(CE.ID) FROM [dbo].[ContractExpenses] AS CE WHERE CE.ContractId = C.ID) AS TotalEstimatedExpenses,
	                (SELECT COUNT(R.ID) FROM [dbo].[Revenues] AS R WHERE R.ContractId = C.ID) AS TotalRealRevenue,
	                (SELECT COUNT(E.ID) FROM [dbo].[Expenses] AS E WHERE E.ContractId = C.ID) AS TotalRealExpenses
                FROM [dbo].[Contracts] AS C
                WHERE C.ID = @ContractId";

            var pars = new DynamicParameters();
            pars.Add("@ContractId", id);

            var contractSummarySource = await this._baseDapperRepository.QueryAsync<ContractSummaryViewModel>(query, pars);
            if (contractSummarySource.Any())
                contractSummary = contractSummarySource.FirstOrDefault();

            return contractSummary;
        }

        public async Task<ContractReportDetailViewModel> GetContractByContractNumber(string contractNumber)
        {
            var result = new ContractReportDetailViewModel();
            var pars = new DynamicParameters();
            string whereQuery = "";

            whereQuery = " C.ContractNumber = @contractNumber";
            pars.Add("@contractNumber", contractNumber);

            string query = $@"
                                    SELECT 
                                        C.ID,
                                        C.Guid,
                                        C.ContractNumber,
                                        C.Area,
                                        C.NumberOfPeople,
                                        C.BuildingId,
                                        C.CustomerId,
                                        C.ContactSignerId,
                                        C.Status,
                                        C.Description,
                                        C.DaysPerMonth,
                                        C.NumberOfRestrooms,
                                        C.ExpirationDate,
                                        C.UnoccupiedSquareFeets,
                                        CS.Name AS CustomerName,
                                        B.Name AS BuildingName
                                    FROM [dbo].[Contracts] AS C
                                        INNER JOIN Customers CS ON CS.ID = C.CustomerId
                                        INNER JOIN Buildings B ON B.ID = C.BuildingId
                                    WHERE { whereQuery }
                ";

            var payload = await _baseDapperRepository.QueryAsync<ContractReportDetailViewModel>(query, pars);
            result = payload.FirstOrDefault();
            return result;
        }

        public async Task<ContractChildDetailViewModel> GetBudgetDetailsDapperAsync(int? id, Guid? guid, string userEmail = "")
        {
            var result = new ContractChildDetailViewModel();
            string whereQuery = "";
            DynamicParameters pars = new DynamicParameters();
            pars.Add("@UserEmail", userEmail);
            if (guid.HasValue)
            {
                whereQuery = " C.Guid = @ContractGuid";
                pars.Add("@ContractGuid", guid);
            }
            else
            {
                whereQuery = " C.ID = @ContractId";
                pars.Add("@ContractId", id);
            }

            string query = $@"
                SELECT 
                    C.ID,
                    C.Guid,
                    C.ContractNumber,
                    C.Area,
                    C.NumberOfPeople,
                    C.BuildingId,
                    C.CustomerId,
                    C.ContactSignerId,
                    C.Status,
                    C.Description,
                    C.DaysPerMonth,
                    C.NumberOfRestrooms,
                    C.ExpirationDate,
                    C.UnoccupiedSquareFeets,
                    C.ProductionRate,
                    CS.Name AS CustomerName,
                    B.Name AS BuildingName
                FROM [dbo].[Contracts] AS C
                    INNER JOIN Customers CS ON CS.ID = C.CustomerId
                    INNER JOIN Buildings B ON B.ID = C.BuildingId
                WHERE { whereQuery }";

            var contractData = await this._baseDapperRepository.QueryAsync<ContractDetailViewModel>(query, pars);
            result.BudgetDetail = contractData.FirstOrDefault();

            if (guid.HasValue)
            {
                pars.Add("@ContractId", result.BudgetDetail.ID);
            }

            string estimatedRevenueQuery = $@"
                                        SELECT 
                                            CI.ID,
                                            CI.Quantity,
                                            CI.[Description],
                                            CI.OfficeServiceTypeId,
                                            CI.OfficeServiceTypeName,
                                            CI.Rate,
                                            CI.RateType,
                                            CI.RatePeriodicity,
                                            CI.SquareFeet,
                                            CI.Amount,
                                            CI.[Hours],
                                            CI.Rooms,
                                            CI.[DefaultType],
                                            CI.[Order]
                                        FROM [dbo].[ContractItems] AS CI
                                        WHERE CI.ContractId = @ContractId ORDER BY [Order]";
            var estimatedRevenueSource = await this._baseDapperRepository.QueryAsync<ContractItemGridViewModel>(estimatedRevenueQuery, pars);
            if (estimatedRevenueSource.Any())
                result.EstimatedRevenues = estimatedRevenueSource;

            string estimatedExpensesQuery = @"
                                            SELECT  
                                            CE.ID,
                                            CE.Quantity,
                                            CE.Description,
                                            CE.ExpenseTypeName,
                                            CE.ExpenseCategory,
                                            CE.ExpenseSubcategoryId,
                                            CE.ExpenseSubcategoryName,
                                            CE.Rate,
                                            CE.RateType,
                                            CE.RatePeriodicity,
                                            CE.Value,
                                            CE.OverheadPercent,
                                            CASE WHEN ES.ID IS NOT NULL THEN ES.ExpenseTypeId ELSE 0 END AS ExpenseTypeId,
                                            CE.DefaultType
                                            FROM [dbo].[ContractExpenses] AS CE LEFT JOIN [dbo].[ExpenseSubcategories] AS ES ON ES.ID=CE.ExpenseSubcategoryId
                                            WHERE CE.ContractId = @ContractId";

            var estimatedExpensesSource = await this._baseDapperRepository.QueryAsync<ContractExpenseGridViewModel>(estimatedExpensesQuery, pars);
            if (estimatedExpensesSource.Any())
                result.EstimatedExpenses = estimatedExpensesSource;

            string notesQuery = $@"
                SELECT
	                CN.[ID],
	                CN.[ContractId],
	                CN.[EmployeeId],
	                CN.[CreatedDate],
	                CN.[UpdatedDate],
	                CN.[Note],
	                CN.[CreatedBy] AS EmployeeEmail,
	                CONCAT(T.[FirstName], ' ', T.[MiddleName], ' ', T.[LastName]) AS [EmployeeFullName],
                    CASE WHEN CN.[CreatedBy] = @UserEmail THEN 1 ELSE 0 END AS Me
                FROM [dbo].[ContractNotes] AS CN
	                LEFT JOIN [dbo].[Contracts] AS C ON C.[ID] = CN.[ContractId]
	                LEFT OUTER JOIN [dbo].[Employees] AS E ON E.[ID] = CN.[EmployeeId]
	                LEFT OUTER JOIN [dbo].[Contacts] AS T ON T.[ID] = E.[ContactId]
                WHERE CN.[ContractId] = @ContractId";
            var notesSource = await this._baseDapperRepository.QueryAsync<ContractNoteGridViewModel>(notesQuery, pars);
            if (notesSource.Any())
                result.Notes = notesSource;

            string activityLogQuery = $@"
                    SELECT TOP 5
						CL.[ID],
						CL.[CreatedDate],
						CL.[EmployeeId],
						CONCAT(C.FirstName, ' ', C.LastName) AS EmployeeFullName,
						CL.[ActivityType],
						CL.[ChangeLog],
						CL.[ItemLog],
                        AN.*
					FROM [dbo].[ContractActivityLog] AS CL
						LEFT JOIN [dbo].[Employees] E ON E.ID = CL.EmployeeId 
						LEFT JOIN [dbo].[Contacts] C ON E.ContactId = C.ID 
                        OUTER APPLY(
		                    SELECT 
		                    TOP 1 
			                    N.[UpdatedDate] AS [NoteUpdatedDate],
			                    N.[Note] AS [Note],
			                    CONCAT(CC.[FirstName], ' ', CC.[LastName]) AS [NoteEmployeeFullName]
		                    FROM ContractActivityLogNotes N 
			                    LEFT JOIN [dbo].[Employees] CE ON CE.[ID] = CL.[EmployeeId] 
			                    LEFT JOIN [dbo].[Contacts] CC ON CE.[ContactId] = CC.[ID]
		                    WHERE N.[ContractActivityLogId] = CL.[ID]
		                    ORDER BY N.ID DESC
	                    ) AS AN
					WHERE CL.ContractId = @ContractId 
                    ORDER BY CL.CreatedDate DESC";
            var activityLogSource = await this._baseDapperRepository.QueryAsync<ContractActivityLogGridViewModel>(activityLogQuery, pars);
            if (activityLogSource.Any())
                result.ActivityLog = activityLogSource;

            return result;
        }
    }
}
