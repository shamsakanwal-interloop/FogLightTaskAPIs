using FogLightTask.DTOs;
using FogLightTask.Entity;
using FogLightTask.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FogLightTask.Service;

public class OrderDetailAppService : ApplicationService
{
    private readonly IOrderDetailRepository _repository;

    public OrderDetailAppService(IOrderDetailRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<OrderDetailDto>> GetOrderDetailsAsync()
    {
        var data = await _repository.GetOrderDetailsAsync();

        return ObjectMapper.Map<List<OrderDetailView>, List<OrderDetailDto>>(data);
    }
}