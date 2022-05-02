using MGCap.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MGCap.Domain.Enums
{
    public class WorkOrderContactType
    {
        public string Value { get; set; }
        public string Key => this.Value.GenerateSlug();
        public int Priority { get; set; }
        public int ID { get; set; }
        public WorkOrderContactType(string value, int id = -1, int priority = -1)
        {
            this.Value = value;
            this.Priority = priority;
            this.ID = id;
            // Checks if value's slug is equal to any pre-existing contacts
            // and gets the first matching ocurrence's priority in case it exists
            if (priority != -1 && id != -1)
            {
                return;
            }
            var match = WorkOrderUtils.Contacts.FirstOrDefault(c => c.Key == this.Key);
            if (match != null)
            {
                this.Priority = match.Priority;
                this.ID = match.ID;
            }
        }


        #region External
        /// <summary>
        ///     Key: building-owner
        /// </summary>
        public static WorkOrderContactType BuildingOwner = new WorkOrderContactType("Building Owner", 0, 0);

        /// <summary>
        ///     Key: requester
        /// </summary>
        public static WorkOrderContactType Requester = new WorkOrderContactType("Requester", 5, 50);
        /// <summary>
        ///     Key: property-manager
        /// </summary>
        public static WorkOrderContactType PropertyManager = new WorkOrderContactType("Property Manager", 1, 10);
        #endregion

        #region Internal
        /// <summary>
        ///     Key: office-staff
        /// </summary>
        public static WorkOrderContactType OfficeStaff = new WorkOrderContactType("Office Staff", 4, 40);
        /// <summary>
        ///     Key: operations-manager
        /// </summary>
        public static WorkOrderContactType OperationsManager = new WorkOrderContactType("Operations Manager", 2, 20);
        /// <summary>
        ///     Key: supervisor
        /// </summary>
        public static WorkOrderContactType Supervisor = new WorkOrderContactType("Supervisor", 3, 30);

        public override string ToString()
        {
            return this.Value;
        }
        #endregion
    }
}
