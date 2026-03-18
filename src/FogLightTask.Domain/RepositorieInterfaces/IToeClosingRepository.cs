using FogLightTask.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FogLightTask.Repositories;

public interface IToeClosingRepository
{
    Task<List<ToeClosingHourlyView>> GetHourlySummaryAsync(
        DateTime knitDate,
        string shift,
        int tcCostCode);
}
