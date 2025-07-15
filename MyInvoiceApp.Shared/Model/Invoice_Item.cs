using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyInvoiceApp.Shared.Model
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
        [JsonIgnore]
        public virtual Invoice? Invoice { get; set; }
    }
}
