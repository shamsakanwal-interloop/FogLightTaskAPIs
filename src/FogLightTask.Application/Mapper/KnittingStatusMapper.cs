using FogLightTask.DTOs;
using FogLightTask.Entity;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace FogLightTask.Mapper;

[Mapper]
public partial class KnittingStatusMapper : MapperBase<SqlKnittingStatusView, KnittingStatusDto>
{
    public override partial KnittingStatusDto Map(SqlKnittingStatusView source);

    public override partial void Map(SqlKnittingStatusView source, KnittingStatusDto destination);
}