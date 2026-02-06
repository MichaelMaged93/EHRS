using EHRS.Core.DTOs.Dashboard;

namespace EHRS.Core.Abstractions.Queries
{
    public interface IDashboardQueries
    {
        Task<TodayDashboardDto> GetTodayDashboardAsync(int doctorId, string? status, CancellationToken ct);
    }
}
