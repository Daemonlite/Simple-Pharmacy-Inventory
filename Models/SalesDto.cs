

using System.ComponentModel.DataAnnotations;
using pharmacy_management.Entities;

namespace pharmacy_management.Models
{
    public class SaleItemDto
    {
        public Guid DrugId { get; set; }

        public int Quantity { get; set; }

        public decimal PricePerUnit { get; set; } 

        public decimal SubTotal => Quantity * PricePerUnit;
    }

    public class CreateSaleDto
    {
        [Required(ErrorMessage = "Cashier is required")]
        public Guid CashierId { get; set; }

        [Required(ErrorMessage = "Customer is required")]
        public string? Customer { get; set; }

        [Required(ErrorMessage = "Sale items are required")]
        public List<SaleItemDto> Items { get; set; } = [];
    }

    public class SaleResponseDto
    {
        public Guid Id { get; set; }

        public UserDto? Cashier { get; set; }

        public string Customer { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public DateTime Date { get; set; }

        public List<SaleItemDto>? Items { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}