using Microsoft.AspNetCore.Builder;
using FogLightTask;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("FogLightTask.Web.csproj"); 
await builder.RunAbpModuleAsync<FogLightTaskWebTestModule>(applicationName: "FogLightTask.Web");

public partial class Program
{
}
