using System.Security.Claims;
using AdventureRental.Core.DTOs.Customer;

using AdventureRental.Core.Entities;
using AdventureRental.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdventureRental.Api.Controllers;

[ApiController]
[Route("api/v1/customers")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public CustomersController(IUnitOfWork uow) => _uow = uow;

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        var customers = await _uow.Customers.GetAllAsync();
        return Ok(customers.Select(Map));
    }

    [HttpGet("me")]
    public async Task<ActionResult<CustomerDto>> GetMe()
    {
        var oid = User.FindFirstValue("oid");
        var customer = await _uow.Customers.GetByUserIdAsync(oid!);
        return customer == null ? NotFound() : Ok(Map(customer));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetById(int id)
    {
        var customer = await _uow.Customers.GetByIdAsync(id);
        return customer == null ? NotFound() : Ok(Map(customer));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(CreateCustomerRequest request)
    {
        if (await _uow.Customers.GetByEmailAsync(request.Email) != null)
            return Conflict(new { message = "Email already registered." });

        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            UserId = request.UserId
        };
        await _uow.Customers.AddAsync(customer);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, Map(customer));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerDto>> Update(int id, UpdateCustomerRequest request)
    {
        var customer = await _uow.Customers.GetByIdAsync(id);
        if (customer == null) return NotFound();

        if (!User.IsInRole("Admin"))
        {
            var oid = User.FindFirstValue("oid");
            var myCustomer = await _uow.Customers.GetByUserIdAsync(oid!);
            if (myCustomer == null || myCustomer.Id != id)
                return Forbid();
        }

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Phone = request.Phone;
        customer.Address = request.Address;

        _uow.Customers.Update(customer);
        await _uow.SaveChangesAsync();
        return Ok(Map(customer));
    }

    private static CustomerDto Map(Customer c) => new()
    {
        Id = c.Id,
        FirstName = c.FirstName,
        LastName = c.LastName,
        Email = c.Email,
        Phone = c.Phone,
        Address = c.Address,
        UserId = c.UserId,
        CreatedAt = c.CreatedAt
    };
}
