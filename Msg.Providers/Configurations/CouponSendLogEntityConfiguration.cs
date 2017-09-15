using System.Data.Entity.ModelConfiguration;
using Msg.Entities;

namespace Msg.Providers.Configurations
{
    class CouponSendLogEntityConfiguration : EntityTypeConfiguration<CouponSendLogsEntity>
    {
        public CouponSendLogEntityConfiguration()
        {
            this.HasRequired(o => o.User);
            this.HasRequired(o => o.Coupon);
        }
    }
}
