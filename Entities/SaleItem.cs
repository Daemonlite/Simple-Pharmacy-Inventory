

using System.ComponentModel.DataAnnotations.Schema;

namespace pharmacy_management.Entities
{
    public class SaleItem
    {
        public Guid Id { get; set; }

        [ForeignKey("Sale")]
        public Guid SaleId { get; set; }
        public Sale? Sale { get; set; }

        [ForeignKey("Drug")]
        public Guid DrugId { get; set; }
        public Drug? Drug { get; set; }

        public int Quantity { get; set; }

        public decimal PricePerUnit { get; set; }

        [NotMapped]
        public decimal SubTotal => Quantity * PricePerUnit;
    }
}