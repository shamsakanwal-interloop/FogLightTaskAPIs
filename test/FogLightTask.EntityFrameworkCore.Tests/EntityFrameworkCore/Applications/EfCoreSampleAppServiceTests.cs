using FogLightTask.Samples;
using Xunit;

namespace FogLightTask.EntityFrameworkCore.Applications;

[Collection(FogLightTaskTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<FogLightTaskEntityFrameworkCoreTestModule>
{

}
