using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInvoiceApp_Shared.DTO
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public DateTime? Issue_Date { get; set; }
        public DateTime? Due_Date { get; set; }
        public decimal Total { get; set; }
        public ClientDto Client { get; set; }
        public StatusDto Status { get; set; }
        public List<InvoiceItemDto> Items { get; set; } = new();
    }

    public class InvoiceItemDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal Unit_Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Unit_Price * Quantity;
    }

    public class ClientDto
    {
        public Guid Id { get; set; }
        public string Company_Name { get; set; }
    }

    public class StatusDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
