using FogLightTask.Entity;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace FogLightTask.EntityFrameworkCore.DataBase;

[ConnectionStringName("OracleTestDb")]
public class OracleDbContext : AbpDbContext<OracleDbContext>
{
    public OracleDbContext(DbContextOptions<OracleDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<OracleKnittingView>(b =>
        {
            b.HasNoKey();
            b.ToView(null);
        });
    }
}