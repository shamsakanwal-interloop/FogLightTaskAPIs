using FogLightTask.DTOs;
using FogLightTask.Entity;
using FogLightTask.RepositorieInterfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FogLightTask.Service;

public class KnittingStatusAppService: ApplicationService
{
    private readonly IKnittingStatusRepository _repository;

    public KnittingStatusAppService(IKnittingStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<KnittingStatusDto>> GetKnitOrderSummaryAsync()
    {
        var data = await _repository.GetKnitOrderSummaryAsync();

        return ObjectMapper.Map<List<SqlKnittingStatusView>, List<KnittingStatusDto>>(data);
    }
}