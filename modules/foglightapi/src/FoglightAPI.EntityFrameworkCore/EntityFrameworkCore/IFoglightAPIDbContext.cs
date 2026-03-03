using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace FoglightAPI.EntityFrameworkCore;

[ConnectionStringName(FoglightAPIDbProperties.ConnectionStringName)]
public interface IFoglightAPIDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
