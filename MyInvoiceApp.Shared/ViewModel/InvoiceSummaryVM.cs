using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInvoiceApp_Shared.ViewModel
{
    public class InvoiceSummaryVM
    {
        public int Year { get; set; }
        public int? Month { get; set; }
        public decimal Total { get; set; }
    }
}
