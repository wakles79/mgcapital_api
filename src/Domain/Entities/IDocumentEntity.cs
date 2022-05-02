using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Entities
{
    public interface IDocumentEntity<T>
    {
        T Number { get; set; }
    }
}
