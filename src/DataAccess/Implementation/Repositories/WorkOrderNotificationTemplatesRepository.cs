using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class WorkOrderNotificationTemplatesRepository : BaseRepository<WorkOrderNotificationTemplate, int>, IWorkOrderNotificationTemplatesRepository
    {
        public WorkOrderNotificationTemplatesRepository(MGCapDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
