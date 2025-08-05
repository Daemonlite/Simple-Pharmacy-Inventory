

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pharmacy_management.Services;
using pharmacy_management.Models;
using pharmacy_management.Exceptions;

namespace pharmacy_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController(ISalesService salesService) : ControllerBase
    {

        [HttpGet]
        [Authorize(Roles = "Admin,Cashier,Pharmacist")]
        public async Task<ActionResult<List<SaleResponseDto>>> GetAllSalesAsync()
        {
            return Ok(await salesService.GetAllSales());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Cashier,Pharmacist")]
        public async Task<ActionResult<SaleResponseDto>> GetSalesByIdAsync(Guid id)
        {
            return Ok(await salesService.GetSalesById(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<ActionResult<SaleResponseDto>> AddSalesAsync(CreateSaleDto saleData)
        {
            try
            {
                return Ok(await salesService.AddSales(saleData));
            }
            catch (UserNotFoundException ex)
            {

                return NotFound(new { message = ex.Message });
            }
            catch (DrugNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InsufficientDrugQuantityException ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<ActionResult<bool>> DeleteSalesAsync(Guid id)
        {
            try
            {
                await salesService.DeleteSales(id);
                return Ok(new { message = "Sale deleted successfully" });
            }
            catch (SaleNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        



    }
}