using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.DataAccess.Implementation.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class WorkOrderScheduleSettingsRepository : BaseRepository<WorkOrderScheduleSetting, int>, IWorkOrderScheduleSettingsRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        public WorkOrderScheduleSettingsRepository(
            MGCapDbContext dbContext,
            IBaseDapperRepository baseDapperRepository
            ) : base(dbContext)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        public async Task<IEnumerable<string>> ReadAllWorkOrderNumberFromSequence(int workOrderScheduleSettingId)
        {
            string query = $@"
                SELECT
                    CASE 
                        WHEN ISNULL(W.[OriginWorkOrderId], 0) = 0 THEN CAST(W.[Number] AS VARCHAR)
                        ELSE CONCAT(CAST(W.[Number] AS VARCHAR),(SELECT [W].[Number] FROM [dbo].[WorkOrders] AS W WHERE [W].[ID] = W.[OriginWorkOrderId]), '-', [dbo].[ConvertNumberToAlpha](W.[CloneNumber])) 
                    END AS [Number]
                FROM [WorkOrders] AS W
                    INNER JOIN [WorkOrderScheduleSetting] AS S ON S.[ID] = W.[WorkOrderScheduleSettingId]
                WHERE W.[WorkOrderScheduleSettingId] = @SequenceId
                ORDER BY [Number]";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@SequenceId", workOrderScheduleSettingId);

            var rows = await this._baseDapperRepository.QueryAsync<string>(query, parameters);
            return rows;
        }
    }
}
