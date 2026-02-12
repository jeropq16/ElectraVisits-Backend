using ElectraVisits.Application.DTOs.Appointments;
using ElectraVisits.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectraVisits.Api.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _svc;
    public AppointmentsController(IAppointmentService svc) => _svc = svc;

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create(CreateAppointmentDto dto)
        => Ok(await _svc.CreateAsync(dto));

    [HttpGet]
    [Authorize(Roles = "Admin,Operations")]
    public async Task<IActionResult> List([FromQuery] string nic)
        => Ok(await _svc.GetByNicAsync(nic));

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,Operations")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateAppointmentStatusDto dto)
        => Ok(await _svc.UpdateStatusAsync(id, dto));
}