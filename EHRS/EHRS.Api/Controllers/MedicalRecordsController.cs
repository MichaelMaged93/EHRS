using EHRS.Api.Contracts.MedicalRecords;
using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Common;
using EHRS.Core.DTOs.MedicalRecords;
using EHRS.Core.Interfaces;
using EHRS.Core.Requests.MedicalRecords;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class MedicalRecordsController : ControllerBase
    {
        private readonly IMedicalRecordQueries _queries;
        private readonly IMedicalRecordService _service;
        private readonly IWebHostEnvironment _env;

        public MedicalRecordsController(
            IMedicalRecordQueries queries,
            IMedicalRecordService service,
            IWebHostEnvironment env)
        {
            _queries = queries;
            _service = service;
            _env = env;
        }

        // GET /api/MedicalRecords?page=1&pageSize=10&search=...&dateFrom=...&dateTo=...
        [HttpGet]
        public async Task<ActionResult<PagedResult<MedicalRecordListItemDto>>> Get(
            [FromQuery] MedicalRecordQuery query,
            CancellationToken ct)
        {
            int doctorId = 2; // مؤقتًا
            var result = await _queries.GetDoctorMedicalRecordsAsync(doctorId, query, ct);
            return Ok(result);
        }

        // GET /api/MedicalRecords/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MedicalRecordDetailsDto>> GetById(int id, CancellationToken ct)
        {
            var record = await _queries.GetByIdAsync(id, ct);
            if (record is null) return NotFound();
            return Ok(record);
        }

        // GET /api/MedicalRecords/by-appointment/{appointmentId}
        [HttpGet("by-appointment/{appointmentId:int}")]
        public async Task<ActionResult<MedicalRecordDetailsDto>> GetByAppointment(int appointmentId, CancellationToken ct)
        {
            var record = await _queries.GetByAppointmentAsync(appointmentId, ct);
            if (record is null) return NotFound();
            return Ok(record);
        }

        // ✅ POST /api/MedicalRecords  (Form-Data: بيانات + File اختياري)
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<MedicalRecordDetailsDto>> Create(
            [FromForm] CreateMedicalRecordForm form,
            CancellationToken ct)
        {
            int doctorId = 2; // مؤقتًا

            // 1) Create Medical Record
            var request = new CreateMedicalRecordRequest
            {
                PatientId = form.PatientId,
                AppointmentId = form.AppointmentId,
                RecordDateTime = form.RecordDateTime,
                ChiefComplaint = form.ChiefComplaint,
                Diagnosis = form.Diagnosis,
                ClinicalNotes = form.ClinicalNotes,
                Treatment = form.Treatment,
                Radiology = form.Radiology,
                PrescriptionImagePath = null
            };

            var created = await _service.CreateAsync(doctorId, request, ct);

            // 2) Upload prescription (اختياري)
            if (form.File is not null && form.File.Length > 0)
            {
                await using var stream = form.File.OpenReadStream();

                await _service.UploadPrescriptionAsync(
                    created.RecordId,
                    stream,
                    form.File.FileName,
                    _env.WebRootPath,
                    ct);
            }

            // 3) رجّع record النهائي (بعد الرفع لو حصل)
            var finalRecord = await _queries.GetByIdAsync(created.RecordId, ct);
            if (finalRecord is null)
                return CreatedAtAction(nameof(GetById), new { id = created.RecordId }, created);

            return CreatedAtAction(nameof(GetById), new { id = finalRecord.RecordId }, finalRecord);
        }

        // (اختياري) Endpoint رفع منفصل - خليه موجود احتياطي
        // POST /api/MedicalRecords/{recordId}/prescription
        //[HttpPost("{recordId:int}/prescription")]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> UploadPrescription(
        //    int recordId,
        //    IFormFile file,
        //    CancellationToken ct)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("File is required.");

        //    await using var stream = file.OpenReadStream();
        //    await _service.UploadPrescriptionAsync(
        //        recordId,
        //        stream,
        //        file.FileName,
        //        _env.WebRootPath,
        //        ct);

        //    return NoContent();
        //}
    }
}
