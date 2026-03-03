using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace FoglightAPI.EntityFrameworkCore;

[ConnectionStringName(FoglightAPIDbProperties.ConnectionStringName)]
public class FoglightAPIDbContext : AbpDbContext<FoglightAPIDbContext>, IFoglightAPIDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */

    public FoglightAPIDbContext(DbContextOptions<FoglightAPIDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureFoglightAPI();
    }
}
