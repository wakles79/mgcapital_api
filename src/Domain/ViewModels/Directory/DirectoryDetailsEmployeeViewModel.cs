using MGCap.Domain.Enums;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Contact;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Directory
{
    public class DirectoryDetailsEmployeeViewModel : EntityViewModel, IEntityParentViewModel<DirectoryChildBaseViewModel, DirectoryChildBaseViewModel>
    {
        private string _name;

        public string Name { get => _name.RemoveDuplicatedSpaces(); set => _name = value; }

        public string UserEmail { get; set; }

        public string Department { get; set; }

        public EmployeeRole Role { get; set; }

        public DirectoryContactType Type => DirectoryContactType.Employee;

        public DateTime? Birthday { get; set; }

        public string Notes { get; set; }

        public bool SendNotifications { get; set; }

        public IList<DirectoryChildBaseViewModel> Phones => Children1 as IList<DirectoryChildBaseViewModel>;

        public IList<DirectoryChildBaseViewModel> Emails => Children2 as IList<DirectoryChildBaseViewModel>;

        public IList<DirectoryChildBaseViewModel> Children2 { get; set; }

        public IList<DirectoryChildBaseViewModel> Children1 { get; set; }
    }
}
