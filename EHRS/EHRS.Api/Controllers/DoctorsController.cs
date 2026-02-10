using EHRS.Core.DTOs.Doctors;
using EHRS.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _service;

    public DoctorsController(IDoctorService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<DoctorResponse>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{doctorId:int}")]
    public async Task<ActionResult<DoctorResponse>> GetById(int doctorId)
    {
        var doctor = await _service.GetByIdAsync(doctorId);
        return doctor is null ? NotFound() : Ok(doctor);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateDoctorRequest request)
    {
        var id = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { doctorId = id }, new { doctorId = id });
    }

    [HttpPut("{doctorId:int}")]
    public async Task<ActionResult> Update(int doctorId, [FromBody] UpdateDoctorRequest request)
    {
        var ok = await _service.UpdateAsync(doctorId, request);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{doctorId:int}")]
    public async Task<ActionResult> Delete(int doctorId)
    {
        var ok = await _service.DeleteAsync(doctorId);
        return ok ? NoContent() : NotFound();
    }
}
