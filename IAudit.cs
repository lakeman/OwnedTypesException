using System;
using System.Collections.Generic;
using System.Text;

namespace OwnedTypes
{
    public interface IAudit
    {
        Audit Audit { get; set; }
    }
}
