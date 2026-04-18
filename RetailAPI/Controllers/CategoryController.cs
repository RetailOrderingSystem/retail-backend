// Controllers/CategoryController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailAPI.DTOs.ProductDTOs;
using RetailAPI.Services;

namespace RetailAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        public CategoryController(CategoryService categoryService) => _categoryService = categoryService;

        // GET api/category  (public)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cats = await _categoryService.GetAllAsync();
            return Ok(cats);
        }

        // GET api/category/{id}  (public)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cat = await _categoryService.GetByIdAsync(id);
            if (cat == null) return NotFound();
            return Ok(cat);
        }

        // POST api/category  (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var cat = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = cat.CategoryId }, cat);
        }

        // PUT api/category/{id}  (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            var cat = await _categoryService.UpdateAsync(id, dto);
            if (cat == null) return NotFound();
            return Ok(cat);
        }

        // DELETE api/category/{id}  (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _categoryService.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Category deleted" });
        }
    }
}
