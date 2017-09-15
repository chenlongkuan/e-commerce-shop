using System.Data.Entity.ModelConfiguration;
using Msg.Entities;

namespace Msg.Providers.Configurations
{
    class NotifiesEntityConfiguration:EntityTypeConfiguration<NotifiesEntity>
    {
        public NotifiesEntityConfiguration()
        {
            this.HasRequired(o => o.User);
        }
    }
}
