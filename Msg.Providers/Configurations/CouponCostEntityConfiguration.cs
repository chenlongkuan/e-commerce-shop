using System.Data.Entity.ModelConfiguration;
using Msg.Entities;

namespace Msg.Providers.Configurations
{
    class CouponCostEntityConfiguration : EntityTypeConfiguration<CouponCostLogsEntity>
    {
        public CouponCostEntityConfiguration()
        {
            this.HasRequired(o => o.User);
            this.HasRequired(o => o.SendLog);
            this.HasRequired(o => o.Order).WithOptional(o => o.CouponCostLogs);
        }
    }
}
