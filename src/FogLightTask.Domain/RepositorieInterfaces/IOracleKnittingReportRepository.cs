using FogLightTask.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FogLightTask.Repositories;

public interface IOracleKnittingReportRepository
{
    Task<List<OracleKnittingView>> GetOrderDetailsAsync(List<(int OrdNo, int DelNo)> orders);
}