using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MGCap.Domain.ViewModels.PreCalendar
{
    public class PreCalendarUpdateViewModel : PreCalendarBaseViewModel, IEntityParentViewModel<PreCalendarTaskUpdateViewModel>
    {
        public IEnumerable<PreCalendarTaskUpdateViewModel> Tasks { get => _children1?.OrderBy(t => t.CreatedDate); set { _children1 = value as IList<PreCalendarTaskUpdateViewModel>; } }

        protected IList<PreCalendarTaskUpdateViewModel> _children1;

        public IList<PreCalendarTaskUpdateViewModel> Children1 { get => _children1; set { _children1 = value; } }

    }
}
