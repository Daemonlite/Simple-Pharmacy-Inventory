
using System.ComponentModel.DataAnnotations;

namespace pharmacy_management.Models
{
    public class DrugsCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        public Guid CategoryId { get; set; }
    }
    public class DrugsListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }
        public string Description { get; set; } = string.Empty;

        public CategoryDto? Category { get; set; }

        public DateTime CreateadAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}