

using pharmacy_management.Data;
using pharmacy_management.Models;
using pharmacy_management.Exceptions;
using Microsoft.EntityFrameworkCore;
using pharmacy_management.Entities;
namespace pharmacy_management.Services
{
    public class CategoryService(AppDbContext context) : ICategoryService
    {
        public async Task<List<CategoryListDto>> GetAllCategoriesAsync()
        {
            return await context.Categories
                .Select(c => new CategoryListDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<List<CategoryListDto>> GetCategoryByIdAsync(Guid id)
        {
            return await context.Categories
                  .Where(c => c.Id == id)
                  .Select(c => new CategoryListDto
                  {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt
                  })
                  .ToListAsync();
        }

        public async Task<CategoryListDto> CreateCategoryAsync(CategoryCreateDto category)
        {
            var existingCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == category.Name);

            if (existingCategory != null)
            {
                throw new CategoryAlreadyExistsException(category.Name);
            }

            var newCategory = new Category
            {
                Name = category.Name,
                Description = category.Description ?? ""
            };

            await context.Categories.AddAsync(newCategory);
            await context.SaveChangesAsync();
            return new CategoryListDto
            {
                Id = newCategory.Id,
                Description = newCategory.Description,
                Name = newCategory.Name,
                CreatedAt = newCategory.CreatedAt
            };
            
        }

        public async Task<CategoryListDto> UpdateCategoryAsync(Guid id, CategoryCreateDto categoryData)
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id) ?? throw new CategoryNotFoundException(id);

            category.Name = categoryData.Name;
            category.Description = categoryData.Description ?? "";

            await context.SaveChangesAsync();
            return new CategoryListDto
            {
                Id = category.Id,
                Description = category.Description,
                Name = category.Name,
                CreatedAt = category.CreatedAt
            };
            
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await context.Categories.FindAsync(id);

            if (category != null)
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return true;
            }
            throw new CategoryNotFoundException(id);
        }
    }
}