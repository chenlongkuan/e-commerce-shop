using System.Data.Entity.ModelConfiguration;
using Msg.Entities;

namespace Msg.Providers.Configurations
{
    class UserRoleEntityConfiguration : EntityTypeConfiguration<UserRoleEntity>
    {
        public UserRoleEntityConfiguration()
        {
            //this.HasMany(o => o.UserRoleMaps).WithRequired(o => o.Role);
         
        }
    }
}
