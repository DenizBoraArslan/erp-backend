using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Domain.Enums
{
    public enum InvoiceStatus
    {
        Draft = 0,
        Issued = 1,
        PartiallyPaid = 2,
        Paid = 3,
        Cancelled = 4
    }
}
