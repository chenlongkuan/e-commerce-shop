using System.Data.Entity.ModelConfiguration;
using Msg.Entities;

namespace Msg.Providers.Configurations
{
    class SchoolEntityConfiguration:EntityTypeConfiguration<SchoolEntity>
    {
        public SchoolEntityConfiguration()
        {
            this.HasRequired(o => o.Region).WithMany(o=>o.Schools);
        }
    }
}
