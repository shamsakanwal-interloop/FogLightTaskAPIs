using FogLightTask.Entity;
using FogLightTask.EntityFrameworkCore.DataBase;
using FogLightTask.RepositorieInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace FogLightTask.EntityFrameworkCore.Repositories;

public class SqlKnittingStatusRepository : ITransientDependency, IKnittingStatusRepository
{
    private readonly IDbContextProvider<ProductionDbContext> _dbContextProvider;

    public SqlKnittingStatusRepository(IDbContextProvider<ProductionDbContext> dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }

    public async Task<List<SqlKnittingStatusView>> GetKnitOrderSummaryAsync()
    {
        var dbContext = await _dbContextProvider.GetDbContextAsync();

        var sql = @"
        WITH ActiveOrders AS
        (
            SELECT DISTINCT
                T.OrderHash
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
        ),

        BatchData AS
        (
            SELECT
                B.OrderHash,
                MAX(B.LoadDate) AS LD,
                MAX(CAST(B.TotalReqQty AS BIGINT)) AS ReqKnit,
                SUM(CAST(B.KnittedQty AS BIGINT) - CAST(B.KnitDefects AS BIGINT)) AS Knitted
            FROM [fl-prod-interloop-orders-batches] B
            GROUP BY B.OrderHash
        )

        SELECT
            B.LD,                  
            O.OrderNo,        
            O.DelNo AS D,               
            B.ReqKnit,                     
            B.Knitted,              
            (B.ReqKnit - B.Knitted) AS BalKnit,   
            CASE
                WHEN B.Knitted >= B.ReqKnit THEN 'KNITTED'
                ELSE 'PENDING'
            END AS KnitStatus

        FROM ActiveOrders A
        INNER JOIN [fl-prod-orders] O
            ON O.OrderHash = A.OrderHash
        LEFT JOIN BatchData B
            ON B.OrderHash = A.OrderHash
        order by OrderNo;
";

      var result =  await dbContext.Database
            .SqlQueryRaw<SqlKnittingStatusView>(sql)
            .ToListAsync();
        
        return result;
    }  
}