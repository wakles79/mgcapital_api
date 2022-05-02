using System.Collections.Generic;
using System.Linq;

namespace MGCap.Domain.Utils
{
    public class DataSource<T>
    {
        private int? _count;

        public int Count
        {
            get => _count ?? Payload.Count();
            set => _count = value;
        }

        public IEnumerable<T> Payload { get; set; }

        public DataSource()
        {
            this.Payload = new List<T>();
        }
    }
}
