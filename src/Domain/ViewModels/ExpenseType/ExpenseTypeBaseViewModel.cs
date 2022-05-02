using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.Enums;

namespace MGCap.Domain.ViewModels.ExpenseType
{
    public class ExpenseTypeBaseViewModel : EntityViewModel
    {
        public ExpenseCategory ExpenseCategory { get; set; }

        public string Description { get; set; }

        public bool Status { get; set; }
    }
}
