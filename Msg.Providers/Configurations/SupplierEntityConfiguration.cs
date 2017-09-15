using System.Data.Entity.ModelConfiguration;
using Msg.Entities;

namespace Msg.Providers.Configurations
{
    class SupplierEntityConfiguration : EntityTypeConfiguration<SuppliersEntity>
    {
        public SupplierEntityConfiguration()
        {
            this.HasRequired(o => o.User);
            this.HasMany(o => o.ProductsApply).WithRequired(o => o.Supplier);
        }

    }
}
