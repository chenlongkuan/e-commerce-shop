using System.Data.Entity.ModelConfiguration;
using Msg.Entities;

namespace Msg.Providers.Configurations
{
    class OrdersEntityConfiguration:EntityTypeConfiguration<OrdersEntity>
    {
        public OrdersEntityConfiguration()
        {
            this.HasRequired(o => o.User).WithMany(o=>o.Orders);
            this.HasMany(o => o.Items).WithRequired(o => o.Order);
            this.HasRequired(o => o.Address);
            
        }
    }
}
