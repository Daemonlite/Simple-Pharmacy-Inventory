
using pharmacy_management.Models;

namespace pharmacy_management.Services
{
    public interface ISalesService
    {
        Task<List<SaleResponseDto>> GetAllSales();
        Task<SaleResponseDto> AddSales(CreateSaleDto salesDto);
        Task<SaleResponseDto?> GetSalesById(Guid id);
        Task<bool> DeleteSales(Guid id);
    }
}