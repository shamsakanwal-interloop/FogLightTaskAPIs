using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace FogLightTask.Pages;

[Collection(FogLightTaskTestConsts.CollectionDefinitionName)]
public class Index_Tests : FogLightTaskWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
