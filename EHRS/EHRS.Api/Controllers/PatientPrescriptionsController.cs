using System.IO;
using EHRS.Core.Abstractions.Queries;
using EHRS.Core.DTOs.Prescriptions;
using EHRS.Core.Requests.Prescriptions;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PatientPrescriptionsController : ControllerBase
{
    private readonly IPatientPrescriptionsQueries _queries;
    private readonly IWebHostEnvironment _env;

    // TODO: Replace with patientId extracted from JWT later
    private const int patientId = 10;

    public PatientPrescriptionsController(IPatientPrescriptionsQueries queries, IWebHostEnvironment env)
    {
        _queries = queries;
        _env = env;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PatientPrescriptionsPagedResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PatientPrescriptionsPagedResultDto>> Get([FromQuery] GetPatientPrescriptionsRequest request)
    {
        var tab = (request.Tab ?? string.Empty).Trim().ToLowerInvariant();
        if (tab is not ("active" or "past"))
            return BadRequest("Invalid tab. Allowed values: active, past.");

        request.Tab = tab;

        var result = await _queries.GetPatientPrescriptionsAsync(patientId, request);
        return Ok(result);
    }

    [HttpGet("{recordId:int}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download([FromRoute] int recordId)
    {
        var fileRef = await _queries.GetPrescriptionFileRefAsync(patientId, recordId);
        if (fileRef is null)
        {
            return NotFound(new
            {
                message = "Prescription not found or not owned by current patient, or no prescription attached.",
                patientId,
                recordId
            });
        }

        var rawPath = (fileRef.PrescriptionPath ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(rawPath))
        {
            return NotFound(new
            {
                message = "Prescription path is empty in database.",
                patientId,
                recordId
            });
        }

        var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");

        // 1) absolute path: use as-is
        string candidatePath;
        if (Path.IsPathRooted(rawPath))
        {
            candidatePath = rawPath;
        }
        else
        {
            // normalize slashes
            var normalized = rawPath.Replace('\\', '/').TrimStart('/');

            // If DB stores "uploads/..." => map to wwwroot/uploads/...
            candidatePath = Path.Combine(webRoot, normalized.Replace('/', Path.DirectorySeparatorChar));
        }

        if (!System.IO.File.Exists(candidatePath))
        {
            // fallback: if only filename stored, try default folder wwwroot/uploads/prescriptions/
            var fileNameOnly = Path.GetFileName(rawPath);
            var fallback = Path.Combine(webRoot, "uploads", "prescriptions", fileNameOnly);

            if (System.IO.File.Exists(fallback))
                candidatePath = fallback;
            else
            {
                return NotFound(new
                {
                    message = "Prescription file not found on disk.",
                    patientId,
                    recordId,
                    storedPath = rawPath,
                    resolvedPathTried = candidatePath,
                    fallbackTried = fallback
                });
            }
        }

        var content = await System.IO.File.ReadAllBytesAsync(candidatePath);

        var ext = Path.GetExtension(candidatePath).ToLowerInvariant();
        var contentType = ext switch
        {
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            _ => "application/octet-stream"
        };

        var fileName = $"prescription_record_{fileRef.RecordId}{ext}";
        return File(content, contentType, fileName);
    }
}
