using FogLightTask.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FogLightTask.Repositories;
public interface IKnittingReportRepository
{
    Task<List<SqlKnittingView>> GetProductionReportAsync(int CostCode);
}