using System.Data.Entity.ModelConfiguration;
using Msg.Entities;

namespace Msg.Providers.Configurations
{
    class CommentsEntityConfiguration : EntityTypeConfiguration<CommentsEntity>
    {
        public CommentsEntityConfiguration()
        {
            this.HasRequired(o => o.User);
            this.HasOptional(o => o.Goods).WithMany(o=>o.Comments);
            this.HasOptional(o => o.CreditGoods).WithMany(o => o.Comments);
        }
    }
}
