using Microsoft.EntityFrameworkCore;
using MyInvoiceApp.Shared.Model;
using MyInvoiceApp.Shared.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyInvoiceApp_Shared.ViewModel
{
    public class InvoiceVM
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public DateTime? Issue_Date { get; set; }
        public DateTime? Due_Date { get; set; }
        public decimal Total { get; set; }
        public InvoiceClientVM Client { get; set; }
        public InvoiceStatusVM Status { get; set; }
        public List<Invoice_ItemVM> Items { get; set; } = new();
    }

    public class Invoice_ItemVM
    {
        public string Description { get; set; }
        public decimal Unit_Price { get; set; }
        public int Quantity { get; set; }
    }

    public class InvoiceClientVM
    {
        public string Name { get; set; }
    }

    public class InvoiceStatusVM
    {
        public string Name { get; set; }
    }
}
