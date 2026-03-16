using FogLightTask.DTOs;
using FogLightTask.Entity;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace FogLightTask.Mapper;

[Mapper]
public partial class KnittingReportMapper : MapperBase<SqlKnittingView, KnittingReportDto>
{
    public override partial KnittingReportDto Map(SqlKnittingView source);

    public override partial void Map(SqlKnittingView source, KnittingReportDto destination);
}