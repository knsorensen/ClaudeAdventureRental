using AdventureRental.Core.DTOs.Category;
using AdventureRental.Core.Entities;
using AdventureRental.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdventureRental.Api.Controllers;

[ApiController]
[Route("api/v1/equipment-categories")]
public class EquipmentCategoriesController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public EquipmentCategoriesController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EquipmentCategoryDto>>> GetAll()
    {
        var categories = await _uow.EquipmentCategories.GetAllAsync();
        return Ok(categories.Select(Map));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EquipmentCategoryDto>> GetById(int id)
    {
        var category = await _uow.EquipmentCategories.GetByIdAsync(id);
        return category == null ? NotFound() : Ok(Map(category));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<EquipmentCategoryDto>> Create(CreateCategoryRequest request)
    {
        var category = new EquipmentCategory
        {
            Name = request.Name,
            Description = request.Description,
            IconUrl = request.IconUrl
        };
        await _uow.EquipmentCategories.AddAsync(category);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, Map(category));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<EquipmentCategoryDto>> Update(int id, CreateCategoryRequest request)
    {
        var category = await _uow.EquipmentCategories.GetByIdAsync(id);
        if (category == null) return NotFound();

        category.Name = request.Name;
        category.Description = request.Description;
        category.IconUrl = request.IconUrl;

        _uow.EquipmentCategories.Update(category);
        await _uow.SaveChangesAsync();
        return Ok(Map(category));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _uow.EquipmentCategories.GetByIdAsync(id);
        if (category == null) return NotFound();

        _uow.EquipmentCategories.Delete(category);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    private static EquipmentCategoryDto Map(EquipmentCategory c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Description = c.Description,
        IconUrl = c.IconUrl
    };
}
