using System.Security.Claims;
using AdventureRental.Core.DTOs.Reservation;
using AdventureRental.Core.Entities;
using AdventureRental.Core.Enums;
using AdventureRental.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdventureRental.Api.Controllers;

[ApiController]
[Route("api/v1/reservations")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public ReservationsController(IUnitOfWork uow) => _uow = uow;

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetAll()
    {
        var reservations = await _uow.Reservations.GetAllAsync();
        return Ok(reservations.Select(Map));
    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetMine()
    {
        var oid = User.FindFirstValue("oid");
        var customer = await _uow.Customers.GetByUserIdAsync(oid!);
        if (customer == null) return Ok(Array.Empty<ReservationDto>());

        var reservations = await _uow.Reservations.GetByCustomerIdAsync(customer.Id);
        return Ok(reservations.Select(Map));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReservationDto>> GetById(int id)
    {
        var reservation = await _uow.Reservations.GetByIdAsync(id);
        if (reservation == null) return NotFound();

        if (!User.IsInRole("Admin"))
        {
            var oid = User.FindFirstValue("oid");
            var customer = await _uow.Customers.GetByUserIdAsync(oid!);
            if (customer == null || reservation.CustomerId != customer.Id)
                return Forbid();
        }

        return Ok(Map(reservation));
    }

    [HttpPost]
    public async Task<ActionResult<ReservationDto>> Create(CreateReservationRequest request)
    {
        request.StartDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc);
        request.EndDate = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc);

        if (request.StartDate >= request.EndDate)
            return BadRequest(new { message = "EndDate must be after StartDate." });

        var oid = User.FindFirstValue("oid");
        var customer = await _uow.Customers.GetByUserIdAsync(oid!);
        if (customer == null)
        {
            // Auto-create Customer record from Entra token claims
            var firstName = User.FindFirstValue("given_name") ?? "Unknown";
            var lastName = User.FindFirstValue("family_name") ?? "User";
            var email = User.FindFirstValue("preferred_username")
                ?? User.FindFirstValue("email")
                ?? string.Empty;

            customer = new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserId = oid
            };
            await _uow.Customers.AddAsync(customer);
            await _uow.SaveChangesAsync();
        }

        var items = new List<ReservationItem>();
        decimal total = 0;
        var days = (int)(request.EndDate - request.StartDate).TotalDays;

        foreach (var itemReq in request.Items)
        {
            var equipment = await _uow.Equipment.GetByIdAsync(itemReq.EquipmentId);
            if (equipment == null)
                return BadRequest(new { message = $"Equipment {itemReq.EquipmentId} not found." });

            var available = await _uow.Equipment.GetAvailableUnitsAsync(
                itemReq.EquipmentId, request.StartDate, request.EndDate);

            if (available < itemReq.Quantity)
                return BadRequest(new { message = $"Insufficient availability for equipment '{equipment.Name}'." });

            var lineTotal = equipment.DailyRate * itemReq.Quantity * days;
            items.Add(new ReservationItem
            {
                EquipmentId = itemReq.EquipmentId,
                Quantity = itemReq.Quantity,
                DailyRate = equipment.DailyRate,
                LineTotal = lineTotal
            });
            total += lineTotal;
        }

        var reservation = new Reservation
        {
            CustomerId = customer.Id,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = ReservationStatus.Pending,
            TotalPrice = total,
            Notes = request.Notes,
            Items = items
        };

        await _uow.Reservations.AddAsync(reservation);
        await _uow.SaveChangesAsync();

        reservation = (await _uow.Reservations.GetByIdAsync(reservation.Id))!;
        return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, Map(reservation));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/confirm")]
    public async Task<ActionResult<ReservationDto>> Confirm(int id)
        => await ChangeStatus(id, ReservationStatus.Confirmed, ReservationStatus.Pending);

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult<ReservationDto>> Cancel(int id)
    {
        var reservation = await _uow.Reservations.GetByIdAsync(id);
        if (reservation == null) return NotFound();

        if (!User.IsInRole("Admin"))
        {
            var oid = User.FindFirstValue("oid");
            var customer = await _uow.Customers.GetByUserIdAsync(oid!);
            if (customer == null || reservation.CustomerId != customer.Id)
                return Forbid();
        }

        if (reservation.Status == ReservationStatus.Completed || reservation.Status == ReservationStatus.Cancelled)
            return BadRequest(new { message = "Reservation cannot be cancelled." });

        reservation.Status = ReservationStatus.Cancelled;
        _uow.Reservations.Update(reservation);
        await _uow.SaveChangesAsync();
        return Ok(Map(reservation));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/complete")]
    public async Task<ActionResult<ReservationDto>> Complete(int id)
        => await ChangeStatus(id, ReservationStatus.Completed, ReservationStatus.Confirmed);

    private async Task<ActionResult<ReservationDto>> ChangeStatus(int id, ReservationStatus newStatus, ReservationStatus requiredCurrent)
    {
        var reservation = await _uow.Reservations.GetByIdAsync(id);
        if (reservation == null) return NotFound();

        if (reservation.Status != requiredCurrent)
            return BadRequest(new { message = $"Reservation must be '{requiredCurrent}' to transition to '{newStatus}'." });

        reservation.Status = newStatus;
        _uow.Reservations.Update(reservation);
        await _uow.SaveChangesAsync();
        return Ok(Map(reservation));
    }

    private static ReservationDto Map(Reservation r) => new()
    {
        Id = r.Id,
        CustomerId = r.CustomerId,
        CustomerName = r.Customer != null ? $"{r.Customer.FirstName} {r.Customer.LastName}" : string.Empty,
        StartDate = r.StartDate,
        EndDate = r.EndDate,
        Status = r.Status,
        TotalPrice = r.TotalPrice,
        Notes = r.Notes,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
        Items = r.Items.Select(i => new ReservationItemDto
        {
            Id = i.Id,
            EquipmentId = i.EquipmentId,
            EquipmentName = i.Equipment?.Name ?? string.Empty,
            Quantity = i.Quantity,
            DailyRate = i.DailyRate,
            LineTotal = i.LineTotal
        })
    };
}
