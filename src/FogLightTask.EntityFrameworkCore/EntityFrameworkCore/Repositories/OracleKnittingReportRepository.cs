using FogLightTask.Entity;
using FogLightTask.EntityFrameworkCore.DataBase;
using FogLightTask.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace FogLightTask.EntityFrameworkCore.Repositories;

public class OracleKnittingReportRepository : IOracleKnittingReportRepository, ITransientDependency
{
    private readonly IDbContextProvider<OracleDbContext> _dbContextProvider;

    public OracleKnittingReportRepository(IDbContextProvider<OracleDbContext> dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }

    public async Task<List<OracleKnittingView>> GetOrderDetailsAsync(List<(int OrdNo, int DelNo)> orders)
    {
        var dbContext = await _dbContextProvider.GetDbContextAsync();

        var ordNos = orders.Select(x => x.OrdNo).Distinct();
        var delNos = orders.Select(x => x.DelNo).Distinct();

        var ordNoList = string.Join(",", ordNos);
        var delNoList = string.Join(",", delNos);

        var sql = $@"
SELECT A.PAIR_CODE AS PairCode,
       A.CUST_SIZE AS CustSize,
       C.SIDE AS Side,
       D.ART_FULL_DESC AS ArtFullDesc,
       E.CLR_DESC AS ClrDesc,
       ROUND(SUM(G.DZNS), 2) AS Qty,
       F.ORD_NO AS OrdNo,
       F.DEL_NO AS DelNo
FROM PRD_PAIR_DESC A,
     SMPL_BSCODE_MST B,
     SMPL_PLAN_DTL C,
     SMPL_ARTICLE_MST D,
     SMPL_COLOR_MST E,
     HOMS_ORDERDEL_MST F,
     HOMS_ORDERDEL_DTL G
WHERE A.TRANMSTPK_FK = B.TRANMST_PK
  AND B.TRANDTLPK_FK = C.TRANDTL_PK
  AND C.ART_ID = D.ART_ID
  AND C.CLR_ID = E.CLR_ID
  AND F.TRANMST_PK = G.TRANMSTPK_FK
  AND A.ORD_NO = F.ORD_NO
  AND A.TRANMSTPK_FK = G.BS_TRANMSTPK_FK
  AND B.TRANMST_PK = G.BS_TRANMSTPK_FK
  AND F.ORD_NO IN ({ordNoList})
  AND F.DEL_NO IN ({delNoList})
GROUP BY A.PAIR_CODE, A.CUST_SIZE, C.SIDE, D.ART_FULL_DESC, E.CLR_DESC, F.ORD_NO, F.DEL_NO
";

        var result = await dbContext.Database
    .SqlQueryRaw<OracleKnittingView>(sql)
    .ToListAsync();

        return result;
    }
}