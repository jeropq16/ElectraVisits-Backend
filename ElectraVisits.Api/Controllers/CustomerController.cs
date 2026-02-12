using ElectraVisits.Application.DTOs.Customers;
using ElectraVisits.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectraVisits.Api.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize(Roles = "Admin,Operations")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _svc;
    public CustomersController(ICustomerService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Create(CreateCustomerDto dto)
        => Ok(await _svc.CreateAsync(dto));

    [HttpGet("bynic/{nic}")]
    public async Task<IActionResult> GetByNic(string nic)
    {
        var c = await _svc.GetByNicAsync(nic);
        return c is null ? NotFound() : Ok(c);
    }
}