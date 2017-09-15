using System.Data.Entity.ModelConfiguration;
using Msg.Entities;

namespace Msg.Providers.Configurations
{
    class UserRoleMapEntityConfiguration : EntityTypeConfiguration<UserRoleMapEntity>
    {
        public UserRoleMapEntityConfiguration()
        {
            this.HasRequired(o => o.Role).WithMany(o => o.UserRoleMaps);
            this.HasRequired(o => o.User).WithOptional(o=>o.RoleMap).Map(m=>m.MapKey("User_Id"));
            this.HasMany(o => o.RegionMaping).WithRequired(o => o.RoleMap);
        }
    }
}
