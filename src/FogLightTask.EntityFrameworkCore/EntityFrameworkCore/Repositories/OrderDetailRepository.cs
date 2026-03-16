using FogLightTask.Entity;
using FogLightTask.EntityFrameworkCore.DataBase;
using FogLightTask.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;

namespace FogLightTask.EntityFrameworkCore.Repositories;

public class OrderDetailRepository : IOrderDetailRepository
{
    private readonly IDbContextProvider<OracleDbContext> _dbContextProvider;

    public OrderDetailRepository(IDbContextProvider<OracleDbContext> dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }

    public async Task<List<OrderDetailView>> GetOrderDetailsAsync()
    {
        var dbContext = await _dbContextProvider.GetDbContextAsync();

        var sql = @"
        SELECT 
            A.CUST_SIZE AS CustSize,
            C.SIDE AS Side
        FROM PRD_PAIR_DESC A,
             SMPL_BSCODE_MST B,
             SMPL_PLAN_DTL C
        WHERE A.TRANMSTPK_FK = B.TRANMST_PK
          AND B.TRANDTLPK_FK = C.TRANDTL_PK
        GROUP BY 
            A.CUST_SIZE,
            C.SIDE
        ";

        var result = await dbContext.Database
            .SqlQueryRaw<OrderDetailView>(sql)
            .ToListAsync();

        return result;
    }
}