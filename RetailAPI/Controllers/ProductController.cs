// Controllers/ProductController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailAPI.DTOs.ProductDTOs;
using RetailAPI.Services;

namespace RetailAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly InventoryService _inventoryService;

        public ProductController(ProductService productService, InventoryService inventoryService)
        {
            _productService = productService;
            _inventoryService = inventoryService;
        }

        // GET api/product  (public - with filters)
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductFilterDto filter)
        {
            var result = await _productService.GetProductsAsync(filter);
            return Ok(result);
        }

        // GET api/product/{id}  (public)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound(new { message = "Product not found" });
            return Ok(product);
        }

        // POST api/product  (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
        }

        // PUT api/product/{id}  (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var product = await _productService.UpdateProductAsync(id, dto);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // DELETE api/product/{id}  (Admin only - soft delete)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Product deleted successfully" });
        }

        // GET api/product/{id}/inventory
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/inventory")]
        public async Task<IActionResult> GetInventory(int id)
        {
            var inv = await _inventoryService.GetByProductIdAsync(id);
            if (inv == null) return NotFound();
            return Ok(inv);
        }

        // PUT api/product/{id}/inventory  (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/inventory")]
        public async Task<IActionResult> UpdateInventory(int id, [FromBody] UpdateInventoryDto dto)
        {
            var inv = await _inventoryService.UpdateAsync(id, dto);
            return Ok(inv);
        }

        // GET api/product/inventory/lowstock  (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpGet("inventory/lowstock")]
        public async Task<IActionResult> GetLowStock()
        {
            var list = await _inventoryService.GetLowStockAsync();
            return Ok(list);
        }
    }
}
