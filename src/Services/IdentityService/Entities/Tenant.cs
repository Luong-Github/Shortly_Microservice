namespace IdentityService.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public Guid? StripeCustomerId { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; } // e.g., "enterprise1.example.com"
        public string ConnectionString { get; set; } // Unique DB/schema
    }
}
