using MyInvoiceApp_Shared.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyInvoiceApp.Shared.Model
{
    public class Client
    {
        public Guid Id { get; set; }
        public Guid Company_Id { get; set; } 
        public string Name { get; set; }
        public long SSM_No { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        [ForeignKey("Company_Id")]
        public virtual Company? Company { get; set; }
    }
}
