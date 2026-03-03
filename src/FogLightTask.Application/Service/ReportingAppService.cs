using FogLightTask.DTOs;
using FogLightTask.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.EntityFrameworkCore;

namespace FogLightTask.Service;

public class ReportingAppService : ApplicationService
{
    private readonly IDbContextProvider<FogLightTaskDbContext> _dbContextProvider;

    public ReportingAppService(
        IDbContextProvider<FogLightTaskDbContext> dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }

    //Latest Machine Orders
    public async Task<List<LatestMachineOrderDto>> GetLatestMachineOrdersAsync(int costCode)
    {
        var dbContext = await _dbContextProvider.GetDbContextAsync();

        var sql = @"
    WITH LatestOrder AS
(
    SELECT
        CAST(ot.CostCode AS INT) AS CostCode,
        CAST(ot.SNo AS INT) AS Machine,
        CAST(ot.OrderHash AS NVARCHAR(100)) AS OrderHash,
        CAST(ot.PsCode AS NVARCHAR(50)) AS PsCode,
        CAST(ot.DateCreated AS DATETIME) AS StartTime,
        ROW_NUMBER() OVER
        (
            PARTITION BY ot.CostCode, ot.SNo
            ORDER BY ot.DateCreated DESC
        ) AS rn
    FROM [fl-prod-orders-transactions] ot
    WHERE ot.CostCode = @CostCode
)

SELECT
    CAST(lo.CostCode AS INT) AS CostCode,
    CAST(lo.Machine AS INT) AS Machine,
    CAST(o.OrderNo AS NVARCHAR(50)) AS OrderNo,
    CAST(lo.PsCode AS NVARCHAR(50)) AS PsCode,
    CAST(o.BsCode AS NVARCHAR(50)) AS BsCode,
    CAST(o.PairCode AS NVARCHAR(50)) AS PairCode,
    CAST(o.McSize AS NVARCHAR(50)) AS Size,
    CAST(o.Pattern AS NVARCHAR(100)) AS Pattern,
    CAST(o.BaseColor AS NVARCHAR(100)) AS BaseColor,
    CAST(lo.StartTime AS DATETIME) AS StartTime
FROM LatestOrder lo
JOIN [fl-prod-orders] o
    ON o.OrderHash = lo.OrderHash
WHERE lo.rn = 1
ORDER BY Machine;";

        return await dbContext.Database
            .SqlQueryRaw<LatestMachineOrderDto>(
                sql,
                new SqlParameter("@CostCode", costCode)
            )
            .ToListAsync();
    }


    // Production report between date and costCode
    public async Task<List<ProductionDashboardDto>> GetProductionDashboardAsync(
    DateTime startDate,
    DateTime endDate,
    int costCode)
    {
        var dbContext = await _dbContextProvider.GetDbContextAsync();

        var sql = @"  WITH ProductionReport AS (
    SELECT
        CAST(p.OrderHash AS NVARCHAR(100)) AS OrderHash,
        CAST(p.SNo AS INT) AS SNo,
        CAST(p.KnitDate AS DATETIME) AS KnitDate,
        CAST(p.Shift AS NVARCHAR(20)) AS Shift,
        CAST(p.YarnTransId AS INT) AS YarnTransId,
        CAST(SUM(p.PieceCount)/24.0 AS DECIMAL(18,2)) AS Prod,
        CAST(SUM(p.DefectCount)/24.0 AS DECIMAL(18,2)) AS CGrade
    FROM dbo.[fl-prod-production-syncer-log] p
    WHERE p.KnitDate BETWEEN @StartDate AND @EndDate
      AND p.CostCode = @CostCode
    GROUP BY
        p.OrderHash,
        p.SNo,
        p.KnitDate,
        p.Shift,
        p.YarnTransId
),

OrderQtyReport AS (
    SELECT
        CAST(b.OrderHash AS NVARCHAR(100)) AS OrderHash,
        CAST(SUM(CAST(b.KnittedQty AS BIGINT) - CAST(b.KnitDefects AS BIGINT)) AS BIGINT) AS KnitQty,
        CAST(MAX(CAST(b.TotalReqQty AS BIGINT)) AS BIGINT) AS TotalReqQty
    FROM dbo.[fl-prod-interloop-orders-batches] b
    WHERE OrderHash IN (SELECT DISTINCT OrderHash FROM ProductionReport)
    GROUP BY b.OrderHash
),

LatestOrder AS (
    SELECT *
    FROM (
        SELECT *,
               ROW_NUMBER() OVER (
                   PARTITION BY SNo
                   ORDER BY DateCreated DESC
               ) AS rn
        FROM dbo.[fl-prod-orders-transactions]
        WHERE CostCode = @CostCode
    ) x
    WHERE rn = 1
),

ActiveMachines AS (
    SELECT
        CAST(lo.OrderHash AS NVARCHAR(100)) AS OrderHash,
        CAST(COUNT(DISTINCT lo.SNo) AS INT) AS ActiveMachines
    FROM LatestOrder lo
    GROUP BY lo.OrderHash
)

SELECT
    pr.OrderHash,
    pr.SNo,
    pr.KnitDate,
    pr.Shift,
    pr.YarnTransId,
    pr.Prod,
    pr.CGrade,

    CAST(
        CAST(oq.KnitQty / 24.0 AS DECIMAL(18,2)) AS NVARCHAR(50)
    ) + '/' +
    CAST(
        CAST(oq.TotalReqQty / 24.0 AS DECIMAL(18,2)) AS NVARCHAR(50)
    ) AS KnitReq,

    CAST((oq.TotalReqQty - oq.KnitQty) / 24.0 AS DECIMAL(18,2)) AS Balance,

    CAST(
        CASE
            WHEN oq.TotalReqQty = 0 THEN 'Knitting'
            WHEN (oq.KnitQty * 100.0 / oq.TotalReqQty) >= 100 THEN 'Completed'
            WHEN (oq.KnitQty * 100.0 / oq.TotalReqQty) >= 80 THEN 'Almost Completed'
            ELSE 'Knitting'
        END AS NVARCHAR(50)
    ) AS Status,

    CAST(o.OrderNo + '-' + CAST(o.DelNo AS NVARCHAR(20)) AS NVARCHAR(100)) AS OrderDel,
    CAST(o.PairCode AS NVARCHAR(50)) AS PairCode,
    CAST(o.Pattern AS NVARCHAR(100)) AS Pattern,
    CAST(o.McSize AS NVARCHAR(50)) AS Size,
    CAST(o.BaseColor AS NVARCHAR(100)) AS Color,

    CAST(ISNULL(am.ActiveMachines, 0) AS INT) AS ActiveMachines

FROM ProductionReport pr
JOIN OrderQtyReport oq
    ON pr.OrderHash = oq.OrderHash
LEFT JOIN ActiveMachines am
    ON pr.OrderHash = am.OrderHash
JOIN dbo.[fl-prod-orders] o
    ON pr.OrderHash = o.OrderHash
ORDER BY
    pr.KnitDate,
    pr.Shift,
    pr.SNo,
    pr.OrderHash;";

        return await dbContext.Database
            .SqlQueryRaw<ProductionDashboardDto>(
                sql,
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate),
                new SqlParameter("@CostCode", costCode)
            )
            .ToListAsync();
    }
    //Toe Closing Hourly Summary
    public async Task<List<ToeClosingHourlyDto>> GetToeClosingHourlyAsync(
        DateTime knitDate,
        string shift,
        int tcCostCode)
    {
        var dbContext = await _dbContextProvider.GetDbContextAsync();

        var sql = @"
       ;WITH BaseData AS
(
    SELECT
        CAST(ul.KnitDate AS DATETIME) AS StitchDate,
        CAST(ob.TcCostCode AS INT) AS TcCostCenter,
        CAST(ul.MachineId AS INT) AS MachineId,
        CAST(ob.BatchNo AS NVARCHAR(50)) AS BatchNo,
        CAST(ob.YearCode AS INT) AS YearCode,
        CAST(ul.EmpCode AS NVARCHAR(50)) AS EmpCode,
        CAST(ul.EmpName AS NVARCHAR(100)) AS EmpName,
        CAST(b.Id AS INT) AS BatchId,
        CAST(ul.PieceCount AS DECIMAL(18,2)) AS PieceCount,

        CASE 
            WHEN ul.Shift = 'A' THEN DATEPART(HOUR, ul.DateCreated) - 5
            WHEN ul.Shift = 'B' THEN DATEPART(HOUR, ul.DateCreated) - 13
            WHEN ul.Shift = 'C' THEN
                CASE 
                    WHEN DATEPART(HOUR, ul.DateCreated) >= 22 
                         THEN DATEPART(HOUR, ul.DateCreated) - 21
                    ELSE DATEPART(HOUR, ul.DateCreated) + 3
                END
        END AS ShiftHour

    FROM [fl-prod-tc-user-logs] ul
    JOIN [fl-prod-tc-batches] b
        ON ul.BatchId = b.Id
    JOIN [fl-prod-interloop-orders-batches] ob
        ON ob.BatchNo = b.BatchNo
       AND ob.OrderHash = b.OrderHash

    WHERE
        ul.KnitDate = @KnitDate
        AND ul.Shift = @Shift
        AND ob.TcCostCode = @TcCostCode
)

SELECT
    StitchDate,
    TcCostCenter,
    MachineId,
    BatchNo,
    YearCode,
    EmpCode,
    EmpName,
    BatchId,

    CAST(SUM(PieceCount) / 24.0 AS DECIMAL(18,2)) AS ShiftProd,

    CAST(SUM(CASE WHEN ShiftHour = 1 THEN PieceCount ELSE 0 END) / 24.0 AS DECIMAL(18,2)) AS H1,
    CAST(SUM(CASE WHEN ShiftHour = 2 THEN PieceCount ELSE 0 END) / 24.0 AS DECIMAL(18,2)) AS H2,
    CAST(SUM(CASE WHEN ShiftHour = 3 THEN PieceCount ELSE 0 END) / 24.0 AS DECIMAL(18,2)) AS H3,
    CAST(SUM(CASE WHEN ShiftHour = 4 THEN PieceCount ELSE 0 END) / 24.0 AS DECIMAL(18,2)) AS H4,
    CAST(SUM(CASE WHEN ShiftHour = 5 THEN PieceCount ELSE 0 END) / 24.0 AS DECIMAL(18,2)) AS H5,
    CAST(SUM(CASE WHEN ShiftHour = 6 THEN PieceCount ELSE 0 END) / 24.0 AS DECIMAL(18,2)) AS H6,
    CAST(SUM(CASE WHEN ShiftHour = 7 THEN PieceCount ELSE 0 END) / 24.0 AS DECIMAL(18,2)) AS H7,
    CAST(SUM(CASE WHEN ShiftHour = 8 THEN PieceCount ELSE 0 END) / 24.0 AS DECIMAL(18,2)) AS H8

FROM BaseData
GROUP BY
    StitchDate,
    TcCostCenter,
    MachineId,
    BatchNo,
    YearCode,
    EmpCode,
    EmpName,
    BatchId

ORDER BY MachineId";

        return await dbContext.Database
            .SqlQueryRaw<ToeClosingHourlyDto>(
                sql,
                new SqlParameter("@KnitDate", knitDate),
                new SqlParameter("@Shift", shift),
                new SqlParameter("@TcCostCode", tcCostCode)
            )
            .ToListAsync();
    }


    public async Task<List<OrderBatchSummaryDto>> GetOrderBatchSummaryAsync(
        int ordNo,
        int delNo)
    {
        var dbContext = await _dbContextProvider.GetDbContextAsync();

        var sql = @"
;WITH OrdersCTE AS (
    SELECT
        CAST(OrderHash AS NVARCHAR(100)) AS OrderHash,
        CAST(OrdNo AS INT) AS OrdNo,
        CAST(DelNo AS INT) AS DelNo,
        CAST(PairCode AS NVARCHAR(50)) AS PairCode,
        CAST(Pattern AS NVARCHAR(100)) AS Pattern,
        CAST(McSize AS NVARCHAR(50)) AS Size,
        CAST(BaseColor AS NVARCHAR(100)) AS Color,
        CAST(BsCode AS NVARCHAR(50)) AS BsCode,
        CAST(Article AS NVARCHAR(100)) AS Article
    FROM dbo.[fl-prod-orders]
    WHERE DelNo = @DelNo
      AND OrdNo = @OrdNo
),

InterloopCTE AS (
    SELECT
        CAST(b.OrderHash AS NVARCHAR(100)) AS OrderHash,
        CAST(b.BatchNo AS INT) AS BatchNo,
        CAST(b.PSCode AS NVARCHAR(50)) AS PSCode,
        CAST(SUM(CAST(b.KnittedQty AS BIGINT) - CAST(b.KnitDefects AS BIGINT)) AS DECIMAL(18,2)) AS KnitQty,
        CAST(SUM(CAST(b.KnitDefects AS BIGINT)) AS DECIMAL(18,2)) AS Defects,
        CAST(MAX(CAST(b.TotalReqQty AS BIGINT)) AS DECIMAL(18,2)) AS TotalReqQty
    FROM dbo.[fl-prod-interloop-orders-batches] b
    WHERE b.BatchNo <> -1
      AND b.OrderHash IN (SELECT OrderHash FROM OrdersCTE)
    GROUP BY b.OrderHash, b.BatchNo, b.PSCode  
),

StitchLogsCTE AS (
    SELECT
        CAST(s.OrderHash AS NVARCHAR(100)) AS OrderHash,
        CAST(s.BatchNo AS INT) AS BatchNo,
        CAST(SUM(s.StitchQty) AS DECIMAL(18,2)) AS StitchingQty
    FROM dbo.[fl-prod-tc-hms-sync-logs] s
    WHERE s.OrderHash IN (SELECT OrderHash FROM OrdersCTE)  
    GROUP BY s.OrderHash, s.BatchNo
),

TCBatchesCTE AS (
    SELECT
        CAST(t.OrderHash AS NVARCHAR(100)) AS OrderHash,
        CAST(t.BatchNo AS INT) AS BatchNo,
        CAST(SUM(t.PiecesDone) AS DECIMAL(18,2)) AS SyncStitching
    FROM dbo.[fl-prod-tc-batches] t
    WHERE t.OrderHash IN (SELECT OrderHash FROM OrdersCTE)  
    GROUP BY t.OrderHash, t.BatchNo
)

SELECT
    o.OrderHash,
    o.OrdNo,
    o.DelNo,
    o.PairCode,
    o.Pattern,
    o.Size,
    o.Color,
    o.BsCode,
    o.Article,

    i.BatchNo,
    i.PSCode,

    CAST(i.TotalReqQty / 24.0 AS DECIMAL(18,2)) AS TotalReqQty,
    CAST(i.KnitQty / 24.0 AS DECIMAL(18,2)) AS Knitting,
    CAST(i.Defects / 24.0 AS DECIMAL(18,2)) AS Defects,
    CAST(ISNULL(s.StitchingQty, 0) / 24.0 AS DECIMAL(18,2)) AS Stitching,
    CAST(ISNULL(tc.SyncStitching, 0) / 24.0 AS DECIMAL(18,2)) AS SyncStitching

FROM OrdersCTE o
LEFT JOIN InterloopCTE i ON i.OrderHash = o.OrderHash
LEFT JOIN StitchLogsCTE s ON s.OrderHash = i.OrderHash AND s.BatchNo = i.BatchNo
LEFT JOIN TCBatchesCTE tc ON tc.OrderHash = i.OrderHash AND tc.BatchNo = i.BatchNo

ORDER BY o.OrdNo, o.DelNo, o.OrderHash, i.BatchNo
";

        return await dbContext.Database
            .SqlQueryRaw<OrderBatchSummaryDto>(
                sql,
                new SqlParameter("@OrdNo", ordNo),
                new SqlParameter("@DelNo", delNo)
            )
            .ToListAsync();
    }


    public async Task<List<ProductionReportDto>> GetProductionReportAsync(DateTime knitDate,string shift,int tcCostCode)
    {
        var dbContext = await _dbContextProvider.GetDbContextAsync();

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
ORDER BY ip.BatchNo, o.PairCode, ed.EmpName";

        return await dbContext.Database
            .SqlQueryRaw<ProductionReportDto>(
                sql,
                new SqlParameter("@KnitDate", knitDate),
                new SqlParameter("@Shift", shift),
                new SqlParameter("@TcCostCode", tcCostCode)
            )
            .ToListAsync();
    }
}