namespace InvoiceApp_2.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public Guid? Role_Id { get; set; }
    }
}
