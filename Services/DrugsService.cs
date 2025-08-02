

using Microsoft.EntityFrameworkCore;
using pharmacy_management.Data;
using pharmacy_management.Models;
using pharmacy_management.Exceptions;
using pharmacy_management.Entities;

namespace pharmacy_management.Services
{
    public class DrugsServices(AppDbContext context) : IDrugsService
    {
        public async Task<List<DrugsListDto>> GetAllDrugs()
        {
            var drugs = context.Drugs
            .Include(d => d.Category)
            .Select(d => new DrugsListDto
            {
                Id = d.Id,
                Name = d.Name,
                Price = d.Price,
                Quantity = d.Quantity,
                Description = d.Description,
                Category = d.Category == null ? null : new CategoryDto
                {
                    Id = d.Category.Id,
                    Name = d.Category.Name
                }
            });

            return await drugs.ToListAsync();
        }

        public async Task<List<DrugsListDto>> GetDrugsById(Guid id)
        {
            var drugs = context.Drugs
            .Where(d => d.Id == id)
            .Select(d => new DrugsListDto
            {
                Id = d.Id,
                Name = d.Name,
                Price = d.Price,
                Quantity = d.Quantity,
                Description = d.Description,
                Category = d.Category == null ? null : new CategoryDto
                {
                    Id = d.Category.Id,
                    Name = d.Category.Name
                }
            });
            
            return await drugs.ToListAsync();
        }

        public async Task<DrugsListDto> AddDrugs(DrugsCreateDto drugsDto)
        {

            var existingDrug = await context.Drugs.FirstOrDefaultAsync(d => d.Name == drugsDto.Name);

            if (existingDrug != null)
            {
                throw new DrugAlreadyExistsException(drugsDto.Name);
            }

            if (drugsDto.Price <= 0 || drugsDto.Quantity <= 0)
            {
                throw new InsufficientDrugQuantityException(drugsDto.Name);
            }

            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == drugsDto.CategoryId);
            if (category == null)
            {
                throw new CategoryNotFoundException();
            }

            var newDrug = new Drug
            {
                Name = drugsDto.Name,
                Price = drugsDto.Price,
                Quantity = drugsDto.Quantity,
                Description = drugsDto.Description,
                CategoryId = drugsDto.CategoryId,
                Category = await context.Categories.FirstOrDefaultAsync(c => c.Id == drugsDto.CategoryId)
            };

            await context.Drugs.AddAsync(newDrug);
            await context.SaveChangesAsync();
            return new DrugsListDto
            {
                Id = newDrug.Id,
                Name = newDrug.Name,
                Price = newDrug.Price,
                Quantity = newDrug.Quantity,
                Description = newDrug.Description,
                Category = newDrug.Category == null ? null : new CategoryDto
                {
                    Id = newDrug.Category.Id,
                    Name = newDrug.Category.Name
                }
            };
        }

        public async Task<DrugsListDto> UpdateDrugs(Guid id, DrugsCreateDto drugsDto)
        {
            var drug = await context.Drugs.FirstOrDefaultAsync(d => d.Id == id) ?? throw new DrugNotFoundException(id);
            drug.Name = drugsDto.Name;
            drug.Price = drugsDto.Price;
            drug.Quantity = drugsDto.Quantity;
            drug.CategoryId = drugsDto.CategoryId;
            drug.Description = drugsDto.Description;
            await context.SaveChangesAsync();
            return new DrugsListDto
            {
                Id = drug.Id,
                Name = drug.Name,
                Price = drug.Price,
                Quantity = drug.Quantity,
                Description = drug.Description,
                Category = drug.Category == null ? null : new CategoryDto
                {
                    Id = drug.Category.Id,
                    Name = drug.Category.Name
                }
            };
        }

        public async Task<bool> DeleteDrugs(Guid id)
        {
            var drug = await context.Drugs.FirstOrDefaultAsync(d => d.Id == id) ?? throw new  DrugNotFoundException(id);
            context.Drugs.Remove(drug);
            await context.SaveChangesAsync();
            return true;
        }

    }
}