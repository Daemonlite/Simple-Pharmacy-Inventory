using pharmacy_management.Models;

namespace pharmacy_management.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryListDto>> GetAllCategoriesAsync();
        Task<List<CategoryListDto>> GetCategoryByIdAsync(Guid id);
        Task<CategoryListDto> CreateCategoryAsync(CategoryCreateDto category);
        Task<CategoryListDto> UpdateCategoryAsync(Guid id, CategoryCreateDto category);
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}