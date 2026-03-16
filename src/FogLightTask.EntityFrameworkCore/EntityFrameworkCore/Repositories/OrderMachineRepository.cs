using FogLightTask.Entity;
using FogLightTask.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace FogLightTask.EntityFrameworkCore.Repositories;

public class OrderMachineRepository : IOrderMachineRepository
{
    private readonly IDbContextProvider<FogLightTaskDbContext> _dbContextProvider;

    public OrderMachineRepository(IDbContextProvider<FogLightTaskDbContext> dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }

    public async Task<List<MachinePlains>> GetOrderMachinesAsync(int costCode)
    {
        var dbContext = await _dbContextProvider.GetDbContextAsync();

        var sql = @"

        WITH OrderDetail AS
(
    SELECT 
        o.OrderHash,
        CAST(o.OrdNo AS VARCHAR(10)) + '-' +
        CAST(o.OrderNo AS VARCHAR(10)) + '-' +
        CAST(o.DelNo AS VARCHAR(10)) + '-' +
        CAST(o.PairCode AS VARCHAR(10)) +
        ' (' + o.BsCode + ')' AS FLOrderNo
    FROM [fl-prod-orders] o
),

OrderTransaction AS
(
    SELECT
        ot.SNo,
        ot.OrderHash,
        ot.CostCode
    FROM [fl-prod-orders-transactions] ot
    WHERE ot.CostCode = @CostCode
    AND EXISTS
    (
        SELECT 1
        FROM [fl-prod-orders] o
        WHERE o.OrderHash = ot.OrderHash
    )
)

SELECT
    od.FLOrderNo,
    ot.SNo,
    ot.CostCode,
    ot.OrderHash
FROM OrderDetail od
INNER JOIN OrderTransaction ot
ON od.OrderHash = ot.OrderHash
        ";

        return await dbContext.Database
            .SqlQueryRaw<MachinePlains>(
                sql,
                new SqlParameter("@CostCode", costCode)
            )
            .ToListAsync();
    }
}
