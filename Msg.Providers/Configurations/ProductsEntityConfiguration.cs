using System.Data.Entity.ModelConfiguration;
using Msg.Entities;

namespace Msg.Providers.Configurations
{
    class ProductsEntityConfiguration:EntityTypeConfiguration<ProductsEntity>
    {
        public ProductsEntityConfiguration()
        {
            this.HasOptional(o => o.Brand).WithMany(o => o.Products);
            this.HasRequired(o => o.Category).WithMany(o => o.Products);
            this.HasOptional(o => o.ApplyEntity).WithOptionalPrincipal(o => o.Product);
        }
    }
}
