using FogLightTask.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FogLightTask.Repositories;

public interface IOrderMachineRepository
{
    Task<List<MachinePlains>> GetOrderMachinesAsync(int costCode);
}