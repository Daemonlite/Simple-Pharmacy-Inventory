

using System.ComponentModel.DataAnnotations.Schema;

namespace pharmacy_management.Entities
{
    public class Sale
    {
        public Guid Id { get; set; }

        [ForeignKey("Cashier")]
        public Guid CashierId { get; set; }
        public User? Cashier { get; set; }
        public string Customer { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<SaleItem> SaleItems { get; set; } = [];
    }
}