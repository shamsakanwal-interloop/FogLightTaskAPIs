using FogLightTask.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FogLightTask.Repositories;

public interface IOrderDetailRepository
{
    Task<List<OrderDetailView>> GetOrderDetailsAsync();
}