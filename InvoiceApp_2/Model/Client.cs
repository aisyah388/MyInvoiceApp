using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceApp_2.Model
{
    public class Client
    {
        public Guid Id { get; set; }
        public string? Company_Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}
