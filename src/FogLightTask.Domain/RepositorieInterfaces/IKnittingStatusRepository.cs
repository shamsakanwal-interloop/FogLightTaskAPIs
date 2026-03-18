using FogLightTask.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FogLightTask.RepositorieInterfaces;

public interface IKnittingStatusRepository
{
    Task<List<SqlKnittingStatusView>> GetKnitOrderSummaryAsync();
}