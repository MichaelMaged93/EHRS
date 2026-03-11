using EHRS.Core.Abstractions.Queries;
using DoctorPatientDtos = EHRS.Core.DTOs.DoctorPatients;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EHRS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorPatientsController : ControllerBase
    {
        private readonly IDoctorPatientQueries _queries;

        public DoctorPatientsController(IDoctorPatientQueries queries)
        {
            _queries = queries;
        }

        [HttpGet("medical-records")]
        public async Task<IActionResult> GetMedicalRecordsBySsn([FromQuery] string ssn)
        {
            DoctorPatientDtos.PatientMedicalHistoryDto? result = await _queries.GetMedicalRecordsBySsnAsync(ssn);
            if (result == null) return NotFound("Patient not found");
            return Ok(result);
        }

        [HttpGet("surgeries")]
        public async Task<IActionResult> GetSurgeriesBySsn([FromQuery] string ssn)
        {
            DoctorPatientDtos.PatientSurgeriesDto? result = await _queries.GetSurgeriesBySsnAsync(ssn);
            if (result == null) return NotFound("Patient not found");
            return Ok(result);
        }
    }
}