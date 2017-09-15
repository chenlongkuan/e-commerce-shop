using System.Data.Entity.ModelConfiguration;
using Msg.Entities;

namespace Msg.Providers.Configurations
{
    class UsersEntityConfiguration:EntityTypeConfiguration<UsersEntity>
    {
        public UsersEntityConfiguration()
        {
            this.HasRequired(o => o.School);
            this.HasMany(o => o.UserAddress).WithRequired(o => o.User);
            this.HasOptional(o => o.Supplier).WithRequired(o => o.User).Map(f=>f.ToTable("SuppliersEntity").MapKey("User_Id"));
        }
    }
}
