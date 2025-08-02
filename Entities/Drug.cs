

using System.ComponentModel.DataAnnotations.Schema;

namespace pharmacy_management.Entities
{
    public class Drug
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string Description { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public DateTime CreateadAt { get; set; }

        public DateTime UpdatedAt { get; set; }


    }
}