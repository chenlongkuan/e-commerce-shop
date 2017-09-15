using System.Data.Entity.ModelConfiguration;
using Msg.Entities;

namespace Msg.Providers.Configurations
{
    class CreditsExchangeLogsEntityConfiguration:EntityTypeConfiguration<CreditsExchangeLogsEntity>
    {
        public CreditsExchangeLogsEntityConfiguration()
        {
            this.HasRequired(o => o.User);
            this.HasRequired(o => o.CreditGoods).WithMany(o => o.ExchangeLogs);
            this.HasOptional(o => o.Address);
        }
    }
}
