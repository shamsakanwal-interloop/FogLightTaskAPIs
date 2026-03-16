using FogLightTask.DTOs;
using FogLightTask.Entity;
using FogLightTask.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FogLightTask.Service;

public class OrderMachineAppService:ApplicationService
{
    private readonly IOrderMachineRepository _repository;

    public OrderMachineAppService(IOrderMachineRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MachinePlains>> GetOrderMachinesAsync(int costCode)
    {
        return await _repository.GetOrderMachinesAsync(costCode);
    }
}
