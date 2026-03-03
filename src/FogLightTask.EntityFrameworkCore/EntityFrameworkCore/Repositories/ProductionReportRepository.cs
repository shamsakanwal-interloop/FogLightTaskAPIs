using FogLightTask.Entity;
using FogLightTask.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace FogLightTask.EntityFrameworkCore.Repositories;

public class ProductionReportRepository: EfCoreRepository<FogLightTaskDbContext, ProductionReportView>, IProductionReportRepository
{
    public ProductionReportRepository(IDbContextProvider<FogLightTaskDbContext> dbContextProvider): base(dbContextProvider)
    {
    }

    public async Task<List<ProductionReportView>> GetProductionReportAsync(  DateTime knitDate,string shift,int tcCostCode)
    {
        var dbContext = await GetDbContextAsync();
        dbContext.Database.SetCommandTimeout(300);

        var sql = @"
WITH UserLogFiltered AS (
    SELECT DISTINCT BatchId
    FROM [fl-prod-tc-user-logs]
    WHERE KnitDate = @KnitDate
      AND Shift = @Shift
),

TCBatchFiltered AS (
    SELECT DISTINCT
        tc.BatchNo,
        tc.OrderHash
    FROM [fl-prod-tc-batches] tc
    INNER JOIN UserLogFiltered ul
        ON ul.BatchId = tc.Id
),

InterloopBase AS (
    SELECT  
        ib.BatchNo,
        ib.OrderHash,
        ib.YearCode,
        ib.PsCode
    FROM [fl-prod-interloop-orders-batches] ib
    INNER JOIN TCBatchFiltered tc
        ON tc.BatchNo = ib.BatchNo
       AND tc.OrderHash = ib.OrderHash
    WHERE ib.TcCostCode = @TcCostCode
),

UniqueBatch AS (
    SELECT DISTINCT
        BatchNo,
        YearCode,
        PsCode
    FROM InterloopBase
),

InterloopPlane AS (
    SELECT
        ib.BatchNo,
        ib.OrderHash,
        SUM(ib.ReqQty)/24.0 AS RequiredQty,
        MAX(ib.LoadDate) AS LoadDate
    FROM [fl-prod-interloop-orders-batches] ib
    INNER JOIN UniqueBatch ub
        ON ub.BatchNo = ib.BatchNo
       AND ub.YearCode = ib.YearCode
       AND ub.PsCode = ib.PsCode
    GROUP BY ib.BatchNo, ib.OrderHash
),

TCBatchPlan AS (
    SELECT
        tc.BatchNo,
        tc.OrderHash,
        SUM(tc.PiecesWasted) AS PiecesWasted,
        SUM(tc.PiecesInvisible) AS PiecesInvisible
    FROM [fl-prod-tc-batches] tc
    RIGHT JOIN InterloopPlane ip
        ON tc.BatchNo = ip.BatchNo
       AND tc.OrderHash = ip.OrderHash
    GROUP BY tc.BatchNo, tc.OrderHash
),

UserLogPlan AS (
    SELECT
        tc.BatchNo,
        tc.OrderHash,
        SUM(ul.PieceCount)/24.0 AS StitchedPieces
    FROM InterloopPlane ip
    INNER JOIN [fl-prod-tc-batches] tc
        ON tc.BatchNo = ip.BatchNo
       AND tc.OrderHash = ip.OrderHash
    INNER JOIN [fl-prod-tc-user-logs] ul
        ON ul.BatchId = tc.Id
    GROUP BY tc.BatchNo, tc.OrderHash
),

HMSPlan AS (
    SELECT
        h.BatchNo,
        h.OrderHash,
        SUM(h.StitchQty) AS StitchedHMS
    FROM InterloopPlane ip
    INNER JOIN [fl-prod-tc-hms-sync-logs] h
        ON h.BatchNo = ip.BatchNo
       AND h.OrderHash = ip.OrderHash
    GROUP BY h.BatchNo, h.OrderHash
),

EmployeeDetailCTE AS (
    SELECT
        h.BatchNo,
        h.OrderHash,
        h.PairCode,
        h.EmpCode,
        h.EmpName,
        h.Shift,
        h.ProdType,
        SUM(h.StitchQty) AS StitchQty
    FROM [fl-prod-tc-hms-sync-logs] h
    INNER JOIN InterloopPlane ip
        ON ip.BatchNo = h.BatchNo
       AND ip.OrderHash = h.OrderHash
    GROUP BY
        h.BatchNo,
        h.OrderHash,
        h.PairCode,
        h.EmpCode,
        h.EmpName,
        h.Shift,
        h.ProdType
),

ActiveMachine AS (
    SELECT
        ip.BatchNo,
        ip.OrderHash,
        ul.MachineId,
        ROW_NUMBER() OVER (
            PARTITION BY ip.BatchNo, ip.OrderHash
            ORDER BY ul.DateCreated DESC
        ) AS rn
    FROM InterloopPlane ip
    INNER JOIN [fl-prod-tc-batches] tc
        ON tc.BatchNo = ip.BatchNo
       AND tc.OrderHash = ip.OrderHash
    INNER JOIN [fl-prod-tc-user-logs] ul
        ON ul.BatchId = tc.Id
)

SELECT 
    CAST(1 AS VARCHAR(10)) AS Id,

    CAST(CAST(o.OrderNo AS VARCHAR(20)) + '-' + CAST(o.DelNo AS VARCHAR(10)) AS VARCHAR(50)) AS OrderNoDelNo,

    CAST(ip.LoadDate AS VARCHAR(30)) AS LoadDate,
    CAST(o.PairCode AS VARCHAR(50)) AS PairCode,
    CAST(ip.BatchNo AS VARCHAR(50)) AS BatchNo,

    CAST(CAST(ip.RequiredQty AS INT) AS VARCHAR(20)) AS Required,
    CAST(CAST(ISNULL(ulp.StitchedPieces,0) AS INT) AS VARCHAR(20)) AS StitchedPieces,
    CAST(CAST(ISNULL(hp.StitchedHMS,0) AS INT) AS VARCHAR(20)) AS StitchedHMS,

    CAST(ISNULL(tcp.PiecesWasted,0) + ISNULL(tcp.PiecesInvisible,0) AS VARCHAR(20)) AS WastedInvisible,

    CAST(CAST(ip.RequiredQty - ISNULL(ulp.StitchedPieces,0) AS INT) AS VARCHAR(20)) AS StitchPendingFL,
    CAST(CAST(ISNULL(ulp.StitchedPieces,0) - ISNULL(hp.StitchedHMS,0) AS INT) AS VARCHAR(20)) AS UnstitchedFL,

    CAST(ed.EmpCode AS VARCHAR(50)) AS EmpCode,
    CAST(ed.EmpName AS VARCHAR(100)) AS EmpName,
    CAST(ed.Shift AS VARCHAR(20)) AS Shift,

    CAST(
        CASE 
            WHEN ed.ProdType = 'F' THEN 'Auto (A)'
            WHEN ed.ProdType = 'M' THEN 'Manual (M)'
            ELSE 'Unknown'
        END 
    AS VARCHAR(20)) AS ProductionType,

    CAST(CAST(ISNULL(ed.StitchQty,0) AS INT) AS VARCHAR(20)) AS EmployeeStitchQty,

    CAST(
        CASE 
            WHEN (ip.RequiredQty - ISNULL(ulp.StitchedPieces,0)) = 0
                THEN 'N/A'
            ELSE ISNULL(CAST(am.MachineId AS VARCHAR(20)), 'N/A')
        END 
    AS VARCHAR(20)) AS Machines

FROM InterloopPlane ip
LEFT JOIN ActiveMachine am
    ON am.BatchNo = ip.BatchNo
   AND am.OrderHash = ip.OrderHash
   AND am.rn = 1
LEFT JOIN TCBatchPlan tcp
    ON tcp.BatchNo = ip.BatchNo
   AND tcp.OrderHash = ip.OrderHash
LEFT JOIN UserLogPlan ulp
    ON ulp.BatchNo = ip.BatchNo
   AND ulp.OrderHash = ip.OrderHash
LEFT JOIN HMSPlan hp
    ON hp.BatchNo = ip.BatchNo
   AND hp.OrderHash = ip.OrderHash
LEFT JOIN [fl-prod-orders] o
    ON o.OrderHash = ip.OrderHash
LEFT JOIN EmployeeDetailCTE ed
    ON ed.BatchNo = ip.BatchNo
   AND ed.OrderHash = ip.OrderHash
   AND ed.PairCode = o.PairCode
ORDER BY ip.BatchNo, o.PairCode, ed.EmpName;";


        var result  = await dbContext.Database
            .SqlQueryRaw<ProductionReportView>(
                sql,
                new SqlParameter("@KnitDate", knitDate),
                new SqlParameter("@Shift", shift),
                new SqlParameter("@TcCostCode", tcCostCode)
            )
            .ToListAsync();

        return result;
    }
}