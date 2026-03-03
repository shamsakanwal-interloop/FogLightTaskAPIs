using FogLightTask.DTOs;
using FogLightTask.Entity;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace FogLightTask;

[Mapper]
public partial class ProductionReportMapper: MapperBase<ProductionReportView, ProductionReportDto>
{
    public override partial ProductionReportDto Map(ProductionReportView source);

    public override partial void Map(ProductionReportView source,ProductionReportDto destination);
}