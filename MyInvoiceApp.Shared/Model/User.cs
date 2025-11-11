using MyInvoiceApp_Shared.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyInvoiceApp.Shared.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Guid Company_Id { get; set; }
        public Guid Role_Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        [ForeignKey("Company_Id")]
        public virtual Company? Company { get; set; }

        [ForeignKey("Role_Id")]
        public virtual Role? Role { get; set; }
    }
}
