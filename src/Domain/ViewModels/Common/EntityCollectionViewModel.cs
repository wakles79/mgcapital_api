using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace MGCap.Domain.ViewModels.Common
{
    public class EntityCollectionViewModel<T>
    {
        [IgnoreDataMember]
        public IEnumerable<T> Collection { get; set; }
    }

    public class EntityIdCollectionViewModel : EntityCollectionViewModel<int>
    {
        [Required]
        public IEnumerable<int> Id
        {
            get
            {
                return this.Collection;
            }
            set
            {
                this.Collection = value;
            }
        }
    }

}
