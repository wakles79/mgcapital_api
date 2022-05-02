using MGCap.Domain.Utils;
using System;
using System.Runtime.Serialization;

namespace MGCap.Domain.ViewModels.Contact
{
    public class ContactGridViewModel : ContactBaseViewModel
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

        public string Phone { get; set; }

        public string Ext { get; set; }

        private string _fullAddress;

        public string FullAddress
        {
            get
            {
                return this._fullAddress.RemoveDuplicatedSpaces();
            }
            set
            {
                this._fullAddress = value;
            }
        }

        public string Email { get; set; }

        public string Type { get; set; }

        public int BuildingId { get; set; }

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
