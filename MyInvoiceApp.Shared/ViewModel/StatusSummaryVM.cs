using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInvoiceApp_Shared.ViewModel
{
    public class StatusSummaryVM
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int InvoiceCount { get; set; }
    }
}
