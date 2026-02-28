using AdventureRental.Core.DTOs.Equipment;
using AdventureRental.Core.Entities;
using AdventureRental.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdventureRental.Api.Controllers;

[ApiController]
[Route("api/v1/equipment")]
public class EquipmentController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public EquipmentController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<ActionResult<PagedEquipmentResult>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? categoryId = null,
        [FromQuery] string? search = null)
    {
        var items = await _uow.Equipment.GetAllAsync(page, pageSize, categoryId, search);
        var total = await _uow.Equipment.CountAsync(categoryId, search);

        return Ok(new PagedEquipmentResult
        {
            Items = items.Select(Map),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EquipmentDto>> GetById(int id)
    {
        var equipment = await _uow.Equipment.GetByIdAsync(id);
        return equipment == null ? NotFound() : Ok(Map(equipment));
    }

    [HttpGet("{id}/availability")]
    public async Task<ActionResult<EquipmentAvailabilityDto>> GetAvailability(
        int id,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var equipment = await _uow.Equipment.GetByIdAsync(id);
        if (equipment == null) return NotFound();

        var available = await _uow.Equipment.GetAvailableUnitsAsync(id, startDate, endDate);
        return Ok(new EquipmentAvailabilityDto
        {
            EquipmentId = id,
            TotalUnits = equipment.TotalUnits,
            AvailableUnits = available,
            StartDate = startDate,
            EndDate = endDate
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<EquipmentDto>> Create(CreateEquipmentRequest request)
    {
        var equipment = new Equipment
        {
            CategoryId = request.CategoryId,
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            DailyRate = request.DailyRate,
            TotalUnits = request.TotalUnits,
            Status = request.Status
        };
        await _uow.Equipment.AddAsync(equipment);
        await _uow.SaveChangesAsync();

        equipment = (await _uow.Equipment.GetByIdAsync(equipment.Id))!;
        return CreatedAtAction(nameof(GetById), new { id = equipment.Id }, Map(equipment));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<EquipmentDto>> Update(int id, UpdateEquipmentRequest request)
    {
        var equipment = await _uow.Equipment.GetByIdAsync(id);
        if (equipment == null) return NotFound();

        equipment.CategoryId = request.CategoryId;
        equipment.Name = request.Name;
        equipment.Description = request.Description;
        equipment.ImageUrl = request.ImageUrl;
        equipment.DailyRate = request.DailyRate;
        equipment.TotalUnits = request.TotalUnits;
        equipment.Status = request.Status;

        _uow.Equipment.Update(equipment);
        await _uow.SaveChangesAsync();
        return Ok(Map(equipment));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var equipment = await _uow.Equipment.GetByIdAsync(id);
        if (equipment == null) return NotFound();

        _uow.Equipment.Delete(equipment);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    private static EquipmentDto Map(Equipment e) => new()
    {
        Id = e.Id,
        CategoryId = e.CategoryId,
        CategoryName = e.Category?.Name ?? string.Empty,
        Name = e.Name,
        Description = e.Description,
        ImageUrl = e.ImageUrl,
        DailyRate = e.DailyRate,
        TotalUnits = e.TotalUnits,
        Status = e.Status,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}
