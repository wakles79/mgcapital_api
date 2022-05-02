using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace MGCap.Domain.ViewModels.WorkOrderBillingReport
{
    public class WorkOrderBillingReportGridViewModel : EntityViewModel, IGridViewModel
    {
        #region Reference fields
        public Guid WorkOrderGuid { get; set; }
        public int WorkOrderId { get; set; }

        /// <summary>
        ///     Refers to Work Order Number
        /// </summary>
        public int Number { get; set; }
        //public string Location { get; set; }
        public string ClonePath { get; set; }
        public int? OriginWorkOrderId { get; set; }
        #endregion

        #region Building info
        public string BuildingCode { get; set; }

        public string BuildingName { get; set; }

        public string Location { get; set; }

        #region Billing Report Csv
        public string BuildingBillingInfo => this.BuildingBillingFullName + Environment.NewLine + this.BuildingBillingEmail;
        #endregion

        public string BuildingBillingFullName { get; set; }
        public string BuildingBillingEmail { get; set; }
        //public string BuildingNoteToBilling { get; set; }
        #endregion 

        #region Task info

        public DateTime WorkOrderCreatedDate { get; set; }
        public int EpochWorkOrderCreatedDate => this.WorkOrderCreatedDate.ToEpoch();
        public string WorkOrderCreatedDateText { get { return String.Format("{0:g}", this.WorkOrderCreatedDate); } }
        // public DateTime TaskCreatedDate { get; set; }
        // public int EpochTaskCreatedDate => this.TaskCreatedDate.ToEpoch();
        public bool IsTaskChecked { get; set; }
        //public DateTime WorkOrderCompletedDate { get; set; }
        //public int EpochWorkOrderCompletedDate => this.WorkOrderCompletedDate.ToEpoch();
        //public string WorkOrderCompletedDateText { get { return String.Format("{0:g}", this.WorkOrderCompletedDate); } }
        //public DateTime CompletedDate { get; set; }
        //public int EpochTaskCompletedDate => this.CompletedDate.ToEpoch();

        public string TaskName { get; set; }

        public string TaskNote { get; set; }

        public string BillingNote { get; set; } // hce013

        //public double ServiceSubTotal => this.ServicePrice * this.Quantity;
        //public double ServiceDiscount => this.ServiceSubTotal * this.DiscountPercentage / 100;
        public string ServiceName { get; set; }
        //  public double ServiceQuantity => this.Quantity;
        public double ServicePrice { get; set; }

        public string ServicePriceText => Convert.ToDecimal(this.ServicePrice).ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-us"));

        // public double ServiceTotal => this.ServicePrice * this.Quantity * (1 - this.DiscountPercentage / 100);
        public string UnitFactor { get; set; }

        // public double DiscountPercentage { get; set; }

        public double TUnitPrice { get; set; }

        public double TaskRate { get; set; }

        public double Quantity { get; set; }

        public double TaskUnitPrice => this.OldVersion ? TUnitPrice : (this.Quantity * this.TaskRate);

        public string TaksUnitPriceText => Convert.ToDecimal(this.TaskUnitPrice).ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-us"));
        #endregion

        #region Extra fields
        public string RequesterEmail { get; set; }
        public string BuildingNoteToBilling { get; set; }
        public DateTime CompletedDate { get; set; }
        public int EpochTaskCompletedDate => this.CompletedDate.ToEpoch();
        public string TaskCompletedDateText { get { return String.Format("{0:g}", this.CompletedDate); } }
        public DateTime WorkOrderCompletedDate { get; set; }
        public int EpochWorkOrderCompletedDate => this.WorkOrderCompletedDate.ToEpoch();
        public string WorkOrderCompletedDateText { get { return String.Format("{0:g}", this.WorkOrderCompletedDate); } }

        private string workOrderDescription;
        public string WorkOrderDescription
        {
            get => this.workOrderDescription.RemoveExtraNewLineCharacters();
            set => this.workOrderDescription = value;
        }

        private string closingNotes;
        public string ClosingNotes
        {
            get => this.closingNotes.RemoveExtraNewLineCharacters();
            set => this.closingNotes = value;
        }

        public bool OldVersion { get; set; }

        public int TicketId { get; set; }

        #endregion

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
