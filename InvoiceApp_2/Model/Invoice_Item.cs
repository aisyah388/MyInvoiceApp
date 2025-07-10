using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceApp_2.Model
{
    public class Invoice_Item
    {
        public Guid Id { get; set; }
        public Guid Invoice_Id { get; set; }
        public string? Description { get; set; }
        [Precision(16, 2)]
        public decimal Unit_Price { get; set; }
        public int Quantity { get; set; }
        [ForeignKey("Invoice_Id")]
        public virtual Invoice? Invoice { get; set; }
    }
}
