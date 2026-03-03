using FogLightTask.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.EntityFrameworkCore;

namespace FogLightTask.Service;

public class ToeClosingAppService : ApplicationService
{
    private readonly IDbContextProvider<FogLightTaskDbContext> _dbContextProvider;

    public ToeClosingAppService(
        IDbContextProvider<FogLightTaskDbContext> dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }

    public async Task<List<ToeClosingHourlyDto>> GetHourlySummaryAsync(
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

        var result = await dbContext.Database
            .SqlQueryRaw<ToeClosingHourlyDto>(
                sql,
                new SqlParameter("@KnitDate", knitDate),
                new SqlParameter("@Shift", shift),
                new SqlParameter("@TcCostCode", tcCostCode)
            )
            .ToListAsync();

        return result;
    }
}