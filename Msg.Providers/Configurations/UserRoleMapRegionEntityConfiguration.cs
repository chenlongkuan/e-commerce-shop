using System.Data.Entity.ModelConfiguration;
using Msg.Entities;

namespace Msg.Providers.Configurations
{
    class UserRoleMapRegionEntityConfiguration : EntityTypeConfiguration<UserRoleMapRegionEntity>
    {
        public UserRoleMapRegionEntityConfiguration()
        {
            this.HasRequired(p => p.Region);
            this.HasRequired(p => p.RoleMap);
        }
    }
}
