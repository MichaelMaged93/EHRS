using EHRS.Core.Abstractions.Queries;
using EHRS.Core.DTOs.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class DashboardController : ControllerBase
    {
        private readonly IDashboardQueries _queries;

        public DashboardController(IDashboardQueries queries)
        {
            _queries = queries;
        }

        [HttpGet("today")]
        public async Task<ActionResult<TodayDashboardDto>> GetToday(
            [FromQuery] string? status,
            CancellationToken ct)
        {
            int doctorId = 2; // مؤقتًا للتست
            var result = await _queries.GetTodayDashboardAsync(doctorId, status, ct);
            return Ok(result);
        }
    }
}
