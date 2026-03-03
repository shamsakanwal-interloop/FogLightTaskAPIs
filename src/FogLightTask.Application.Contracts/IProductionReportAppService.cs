using FogLightTask.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FogLightTask;

public interface IProductionReportAppService : IApplicationService
{
    Task<List<ProductionReportDto>> GetReportAsync(DateTime knitDate, string shift, int TcCostCode);
}