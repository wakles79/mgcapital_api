using MGCap.Domain.Utils;
using System;
using System.Runtime.Serialization;

namespace MGCap.Domain.ViewModels.Employee
{
    public class EmployeeGridViewModel : EmployeeBaseViewModel
    {
        public Guid Guid { get; set; }

        private string _fullName;

        public string FullName
        {
            get
            {
                return this._fullName.RemoveDuplicatedSpaces();
            }
            set
            {
                this._fullName = value;
            }
        }

        public string DepartmentName { get; set; }

        public string Phone { get; set; }

        public string Ext { get; set; }

        public string RoleName { get; set; }

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
