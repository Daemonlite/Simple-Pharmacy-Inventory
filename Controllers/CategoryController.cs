
using pharmacy_management.Services;
using pharmacy_management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using pharmacy_management.Exceptions;

namespace pharmacy_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<CategoryListDto>>> GetAllCategoriesAsync()
        {
            return Ok(await categoryService.GetAllCategoriesAsync());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<List<CategoryListDto>>> GetCategoryByIdAsync(Guid id)
        {
            try
            {
                return Ok(await categoryService.GetCategoryByIdAsync(id));
            }
            catch (CategoryNotFoundException ex)
            {

                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CategoryListDto>> CreateCategoryAsync(CategoryCreateDto category)
        {
            try
            {
                return Ok(await categoryService.CreateCategoryAsync(category));
            }
            catch (CategoryAlreadyExistsException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")] 
        [Authorize]
        public async Task<ActionResult<CategoryListDto>> UpdateCategoryAsync(Guid id, CategoryCreateDto categoryData)
        {
            try
            {
                return Ok(await categoryService.UpdateCategoryAsync(id, categoryData));
            }
            catch (CategoryNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteCategoryAsync(Guid id)
        {
            try
            {    
                await categoryService.DeleteCategoryAsync(id);
                return Ok(new { message = "Category deleted successfully" });
            }
            catch (CategoryNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}