using FogLightTask.Entity;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace FogLightTask.EntityFrameworkCore.DataBase;

[ConnectionStringName("ProductionDb")]
public class ProductionDbContext : AbpDbContext<ProductionDbContext>
{
    public ProductionDbContext(DbContextOptions<ProductionDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ProductionReportView>(b =>
        {
            b.HasNoKey();
            b.ToView(null);
        });

        builder.Entity<SqlKnittingView>(b => { b.HasNoKey(); b.ToView(null); });
    }
}