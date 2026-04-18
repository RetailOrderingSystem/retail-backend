// Services/CategoryService.cs
using RetailAPI.DTOs.ProductDTOs;
using RetailAPI.Models;
using RetailAPI.Repositories;

namespace RetailAPI.Services
{
    public class CategoryService
    {
        private readonly CategoryRepository _repo;
        public CategoryService(CategoryRepository repo) => _repo = repo;

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var cats = await _repo.GetAllAsync();
            return cats.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                IsActive = c.IsActive,
                ProductCount = c.Products.Count
            }).ToList();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var c = await _repo.GetByIdAsync(id);
            if (c == null) return null;
            return new CategoryDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                IsActive = c.IsActive,
                ProductCount = c.Products.Count
            };
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var cat = new Category { Name = dto.Name, Description = dto.Description, ImageUrl = dto.ImageUrl };
            var created = await _repo.CreateAsync(cat);
            return new CategoryDto
            {
                CategoryId = created.CategoryId,
                Name = created.Name,
                Description = created.Description,
                ImageUrl = created.ImageUrl,
                IsActive = created.IsActive
            };
        }

        public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var cat = await _repo.GetByIdAsync(id);
            if (cat == null) return null;
            if (dto.Name != null) cat.Name = dto.Name;
            if (dto.Description != null) cat.Description = dto.Description;
            if (dto.ImageUrl != null) cat.ImageUrl = dto.ImageUrl;
            if (dto.IsActive.HasValue) cat.IsActive = dto.IsActive.Value;
            await _repo.UpdateAsync(cat);
            return new CategoryDto
            {
                CategoryId = cat.CategoryId,
                Name = cat.Name,
                Description = cat.Description,
                ImageUrl = cat.ImageUrl,
                IsActive = cat.IsActive
            };
        }

        public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);
    }
}

