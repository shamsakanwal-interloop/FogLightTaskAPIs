using FogLightTask.Entity;
using FogLightTask.EntityFrameworkCore.DataBase;
using FogLightTask.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace FogLightTask.EntityFrameworkCore;

public class SqlKnittingReportRepository :  IKnittingReportRepository, ITransientDependency
{
    private readonly IDbContextProvider<ProductionDbContext> _dbContextProvider;

    public SqlKnittingReportRepository(IDbContextProvider<ProductionDbContext> dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }

    public async Task<List<SqlKnittingView>> GetProductionReportAsync(int CostCode)
    {
        var dbContext = await _dbContextProvider.GetDbContextAsync();

        var sql = @"
WITH OrderTransaction AS
(
    SELECT DISTINCT
        OrderHash,
        CostCode AS CC,
        PsCode
    FROM [fl-prod-orders-transactions]
    WHERE CostCode = @costCode
),
BatchData AS
(
    SELECT
        B.OrderHash,
        MAX(B.LoadDate) AS LD,
        SUM(CAST(B.KnittedQty AS BIGINT) - CAST(B.KnitDefects AS BIGINT)) AS KnitQty,
        MAX(B.TotalReqQty) AS TotalReqQty
    FROM [fl-prod-interloop-orders-batches] B
    WHERE EXISTS
    (
        SELECT 1
        FROM OrderTransaction A
        WHERE A.OrderHash = B.OrderHash
    )
    GROUP BY B.OrderHash
)
SELECT
    A.OrderHash,
    A.CC AS CostCode,
    A.PsCode,
    O.OrdNo,
    O.PairCode,
    O.Pattern AS Design,
    O.DelNo,
    O.McSize,
    O.BaseColor,
    O.BsCode,
    B.LD,
    B.KnitQty,
    (B.TotalReqQty - B.KnitQty) / 24.0 AS Balance
FROM OrderTransaction A
INNER JOIN [fl-prod-orders] O
    ON O.OrderHash = A.OrderHash
LEFT JOIN BatchData B
    ON B.OrderHash = A.OrderHash
ORDER BY O.OrdNo
";

        var result = await dbContext.Database
            .SqlQueryRaw<SqlKnittingView>(
                sql,
                new SqlParameter("@costCode", CostCode)
            )
            .ToListAsync();

        return result;
    }
}