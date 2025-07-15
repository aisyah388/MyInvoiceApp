using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInvoiceApp_Shared.ViewModel
{
    public class InvoiceVM
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime? Issue_Date { get; set; }
        public DateTime? Due_Date { get; set; }
        public decimal Total { get; set; }
        public string ClientName { get; set; }
        public string StatusName { get; set; }
    }
}
