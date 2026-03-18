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
        WITH ActiveOrders AS
        (
            SELECT Distinct
                T.OrderHash,
                T.CostCode,
                T.PsCode
            FROM [fl-prod-orders-transactions] T
            JOIN
            (
                SELECT
                    SNo,
                    MAX(DateCreated) AS MaxDate
                FROM [fl-prod-orders-transactions]
                GROUP BY SNo
            ) M
            ON T.SNo = M.SNo
            AND T.DateCreated = M.MaxDate
            WHERE T.CostCode = @costCode
        ),
        BatchData AS
        (
            SELECT
                B.OrderHash,
                MAX(B.LoadDate) AS LD,
                SUM(CAST(B.KnittedQty AS BIGINT) - CAST(B.KnitDefects AS BIGINT)) AS KnitQty,
                MAX(CAST(B.TotalReqQty AS BIGINT)) AS TotalReqQty
            FROM [fl-prod-interloop-orders-batches] B
            INNER JOIN ActiveOrders A
                ON A.OrderHash = B.OrderHash
            GROUP BY B.OrderHash
        )
        SELECT
            A.OrderHash,
            A.CostCode,
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
            B.TotalReqQty,
            CAST(B.TotalReqQty - B.KnitQty AS DECIMAL(18,2)) AS Balance
        FROM ActiveOrders A
        INNER JOIN [fl-prod-orders] O
            ON O.OrderHash = A.OrderHash
        LEFT JOIN BatchData B
            ON B.OrderHash = A.OrderHash
        ORDER BY O.OrdNo;
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