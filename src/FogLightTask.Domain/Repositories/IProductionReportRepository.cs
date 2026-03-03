using FogLightTask.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FogLightTask.Repositories;

public interface IProductionReportRepository
{
    Task<List<ProductionReportView>> GetProductionReportAsync(
        DateTime knitDate,
        string shift,
        int tcCostCode);
}