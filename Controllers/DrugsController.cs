

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pharmacy_management.Entities;
using pharmacy_management.Exceptions;
using pharmacy_management.Models;
using pharmacy_management.Services;

namespace pharmacy_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrugsController(IDrugsService drugsService) : ControllerBase
    {

        [HttpGet]
        [Authorize(Roles = "Admin,Cashier,Pharmacist")]
        public async Task<ActionResult<List<DrugsListDto>>> GetAllDrugsAsync()
        {
            return Ok(await drugsService.GetAllDrugs());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Cashier,Pharmacist")]
        public async Task<ActionResult<List<DrugsListDto>>> GetDrugsByIdAsync(Guid id)
        {
            try
            {
                return Ok(await drugsService.GetDrugsById(id));
            }
            catch (DrugNotFoundException ex)
            {

                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DrugsListDto>> CreateDrugsAsync(DrugsCreateDto drugs)
        {
            try
            {
                return Ok(await drugsService.AddDrugs(drugs));
            }
            catch (DrugAlreadyExistsException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InsufficientDrugQuantityException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (CategoryNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DrugsListDto>> UpdateDrugsAsync(Guid id, DrugsCreateDto drugsData)
        {
            try
            {
                return Ok(await drugsService.UpdateDrugs(id, drugsData));
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteDrugsAsync(Guid id)
        {
            try
            {
                await drugsService.DeleteDrugs(id);
                return Ok(new { message = "Drug deleted successfully" });
            }
            catch (DrugNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}