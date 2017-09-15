using System.Data.Entity.ModelConfiguration;
using Msg.Entities;

namespace Msg.Providers.Configurations
{
    class GoodsEntityConfiguration:EntityTypeConfiguration<GoodsEntity>
    {
        public GoodsEntityConfiguration()
        {
            this.HasRequired(o => o.Product).WithMany(o => o.Goods);
           
        }

    }
}
