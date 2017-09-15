using System.Data.Entity.ModelConfiguration;
using Msg.Entities;

namespace Msg.Providers.Configurations
{
    class ProductsApplyEntityConfiguration:EntityTypeConfiguration<ProductsApplyEntity>
    {
        public ProductsApplyEntityConfiguration()
        {
            //this.HasOptional(o => o.Product).WithOptionalPrincipal(o=>o.ApplyEntity);
        }
    }
}
