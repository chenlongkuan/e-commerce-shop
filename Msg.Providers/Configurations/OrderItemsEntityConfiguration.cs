using System.Data.Entity.ModelConfiguration;
using Msg.Entities;

namespace Msg.Providers.Configurations
{

    class OrderItemsEntityConfiguration : EntityTypeConfiguration<OrderItemsEntity>
    {
        public OrderItemsEntityConfiguration()
        {
            this.HasRequired(o => o.Goods);
            this.HasRequired(o => o.Order).WithMany(o => o.Items);
        }
    }
}
