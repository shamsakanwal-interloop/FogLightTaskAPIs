using FogLightTask.DTOs;
using FogLightTask.Entity;
using FogLightTask.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FogLightTask.Service;

public class KnittingReportAppService : ApplicationService
{
    private readonly IKnittingReportRepository _repository;
    private readonly IOracleKnittingReportRepository _oracleRepository;


    public KnittingReportAppService(IKnittingReportRepository repository, IOracleKnittingReportRepository oracleRepository)
    {
        _repository = repository;
        _oracleRepository = oracleRepository;
    }

    public async Task<List<KnittingReportDto>> GetProductionReportAsync(int CostCode)
    {

        var sqlRows = await _repository.GetProductionReportAsync(CostCode);

        var dtos = ObjectMapper.Map<List<SqlKnittingView>, List<KnittingReportDto>>(sqlRows);

        var orderPairs = dtos
            .Where(x => x.OrdNo.HasValue && x.DelNo.HasValue)
            .Select(x => (x.OrdNo!.Value, x.DelNo!.Value))
            .Distinct()
            .ToList();

        var oracleRows = await _oracleRepository.GetOrderDetailsAsync(orderPairs);

        var oracleDict = oracleRows.ToDictionary(
                x => (x.OrdNo, x.DelNo, x.PairCode),
                x => x
                );
        foreach (var dto in dtos)
        {
            if (!dto.OrdNo.HasValue || !dto.DelNo.HasValue)
                continue;

            if (oracleDict.TryGetValue((dto.OrdNo.Value, dto.DelNo.Value, dto.PairCode), out var match))
            {
                dto.CustSize = match.CustSize;
                dto.Side = match.Side;
                dto.ArtFullDesc = match.ArtFullDesc;
                dto.ClrDesc = match.ClrDesc;
                dto.Qty = match.Qty;
            }
        }
        return dtos;
    }
}