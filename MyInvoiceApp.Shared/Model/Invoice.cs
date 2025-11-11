using MyInvoiceApp_Shared.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyInvoiceApp.Shared.Model
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public Guid Company_Id { get; set; }
        public Guid Client_Id { get; set; }
        public string? Number { get; set; }
        public Guid? Status_Id { get; set; }
        public DateTime? Issue_Date { get; set; }
        public DateTime? Due_Date { get; set; }
        public List<Invoice_Item>? Items { get; set; }

        [ForeignKey("Company_Id")]
        public virtual Company? Company { get; set; }

        [ForeignKey("Client_Id")]
        public virtual Client? Client { get; set; }

        [ForeignKey("Status_Id")]
        public virtual Status? Status { get; set; }

        [JsonIgnore]
        [NotMapped]
        public decimal Total { get; set; }
    }
}
