

using Microsoft.EntityFrameworkCore;
using pharmacy_management.Data;
using pharmacy_management.Exceptions;
using pharmacy_management.Models;
using pharmacy_management.Entities;

namespace pharmacy_management.Services
{
    public class SalesService(AppDbContext context) : ISalesService
    {
        public async Task<List<SaleResponseDto>> GetAllSales()
        {
            var sales = await context.Sales
                .Include(s => s.Cashier)
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Drug)
                .Select(s => new SaleResponseDto
                {
                    Id = s.Id,
                    Customer = s.Customer,
                    TotalAmount = s.TotalAmount,
                    CreatedAt = s.CreatedAt,

                    Cashier = s.Cashier == null ? null : new UserDto
                    {
                        Id = s.Cashier.Id,
                        Name = s.Cashier.Name,
                        Email = s.Cashier.Email,
                        Role = s.Cashier.Role.ToString()
                    },

                    Items = s.SaleItems.Select(si => new SaleItemDto
                    {
                        DrugId = si.DrugId,
                        Quantity = si.Quantity,
                        PricePerUnit = si.PricePerUnit,
                    }).ToList()

                })
                .ToListAsync();

            return sales;
        }


        public async Task<SaleResponseDto?> GetSalesById(Guid id)
        {
            return await context.Sales
                .Where(s => s.Id == id)
                .Include(s => s.Cashier)
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Drug)
                .Select(s => new SaleResponseDto
                {
                    Id = s.Id,
                    Customer = s.Customer,
                    TotalAmount = s.TotalAmount,
                    CreatedAt = s.CreatedAt,

                    Cashier = s.Cashier == null ? null : new UserDto
                    {
                        Id = s.Cashier.Id,
                        Name = s.Cashier.Name,
                        Email = s.Cashier.Email,
                        Role = s.Cashier.Role.ToString()
                    },

                    Items = s.SaleItems.Select(si => new SaleItemDto
                    {
                        DrugId = si.DrugId,
                        Quantity = si.Quantity,
                        PricePerUnit = si.PricePerUnit,
                    }).ToList()

                })
                .FirstOrDefaultAsync();

        }

        public async Task<SaleResponseDto> AddSales(CreateSaleDto salesDto)
        {
            ArgumentNullException.ThrowIfNull(salesDto);
            if (salesDto.Items == null || salesDto.Items.Count == 0)
                throw new EmptySalesItemsException();

            // Using a transaction to ensure atomicity
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var cashier = await context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == salesDto.CashierId)
                    ?? throw new UserNotFoundException(salesDto.CashierId.ToString());

                // Get only the drugs involved in this sale
                var drugIds = salesDto.Items.Select(i => i.DrugId).Distinct();
                var drugs = await context.Drugs
                    .Where(d => drugIds.Contains(d.Id))
                    .ToListAsync();

                // Validate and update drug quantities
                foreach (var item in salesDto.Items)
                {
                    var drug = drugs.FirstOrDefault(d => d.Id == item.DrugId)
                        ?? throw new DrugNotFoundException(item.DrugId);

                    if (item.Quantity > drug.Quantity)
                        throw new InsufficientDrugQuantityException(drug.Name);

                    drug.Quantity -= item.Quantity;
                }

                var sale = new Sale
                {
                    Customer = salesDto.Customer ?? string.Empty,
                    CashierId = cashier.Id,
                    TotalAmount = salesDto.Items.Sum(si => si.Quantity * drugs.First(d => d.Id == si.DrugId).Price),
                    SaleItems = [.. salesDto.Items.Select(si => new SaleItem
                    {
                        DrugId = si.DrugId,
                        Quantity = si.Quantity,
                        PricePerUnit = drugs.First(d => d.Id == si.DrugId).Price, // Set price from drug
                    })]
                };

                await context.Sales.AddAsync(sale);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Reload sale with related data for response
                var createdSale = await context.Sales
                    .Include(s => s.SaleItems)
                        .ThenInclude(si => si.Drug)
                    .Include(s => s.Cashier)
                    .FirstOrDefaultAsync(s => s.Id == sale.Id);

                return new SaleResponseDto
                {
                    Id = createdSale!.Id,
                    Customer = createdSale.Customer,
                    TotalAmount = createdSale.SaleItems.Sum(si => si.Quantity * si.PricePerUnit),
                    CreatedAt = createdSale.CreatedAt,
                    Cashier = new UserDto
                    {
                        Id = cashier.Id,
                        Name = cashier.Name,
                        Email = cashier.Email,
                        Role = cashier.Role.ToString()
                    },
                    Items = [.. createdSale.SaleItems.Select(si => new SaleItemDto
                    {
                        DrugId = si.DrugId,
                        Quantity = si.Quantity,
                        PricePerUnit = si.PricePerUnit,
                    })]
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }



        public async Task<bool> DeleteSales(Guid id)
        {
            // Begin transaction
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // Load sale with items and related drugs
                var sale = await context.Sales
                    .Include(s => s.SaleItems)
                        .ThenInclude(si => si.Drug)
                    .FirstOrDefaultAsync(s => s.Id == id)
                    ?? throw new SaleNotFoundException(id);

                // Restore drug quantities
                foreach (var item in sale.SaleItems)
                {
                    if (item.Drug != null)
                    {
                        item.Drug.Quantity += item.Quantity;
                    }
                }

                // Remove the sale and its items
                context.Sales.Remove(sale);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}