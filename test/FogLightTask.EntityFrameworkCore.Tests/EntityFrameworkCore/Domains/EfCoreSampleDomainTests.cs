using FogLightTask.Samples;
using Xunit;

namespace FogLightTask.EntityFrameworkCore.Domains;

[Collection(FogLightTaskTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<FogLightTaskEntityFrameworkCoreTestModule>
{

}
