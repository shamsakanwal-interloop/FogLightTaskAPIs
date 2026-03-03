using FogLightTask.DTOs;
using FogLightTask.Entity;
using FogLightTask.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FogLightTask.Service;

public class ProductionReportAppService : ApplicationService, IProductionReportAppService
{
    private readonly IProductionReportRepository _repository;

    public ProductionReportAppService(IProductionReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProductionReportDto>> GetReportAsync(DateTime knitDate, string shift, int TcCostCode)
    {
        var data = await _repository.GetProductionReportAsync(
            knitDate,
            shift,
            TcCostCode
        );

        return ObjectMapper.Map<
            List<ProductionReportView>,
            List<ProductionReportDto>>(data);
    }
}