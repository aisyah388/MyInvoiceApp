using MyInvoiceApp.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInvoiceApp_Shared.Model
{
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public long SSM_No { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public virtual ICollection<User>? Users { get; set; }
        public virtual ICollection<Client>? Clients { get; set; } 
        public virtual ICollection<Invoice>? Invoices { get; set; } 
    }
}
