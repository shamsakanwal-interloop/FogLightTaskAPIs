using Xunit;

namespace FogLightTask.EntityFrameworkCore;

[CollectionDefinition(FogLightTaskTestConsts.CollectionDefinitionName)]
public class FogLightTaskEntityFrameworkCoreCollection : ICollectionFixture<FogLightTaskEntityFrameworkCoreFixture>
{

}
