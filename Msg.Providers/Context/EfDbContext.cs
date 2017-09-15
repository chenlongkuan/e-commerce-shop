using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using Msg.Entities;

namespace Msg.Providers.Context
{


    public class EfDbContext : DbContext
    {
        public EfDbContext()
            : base("default")
        {
            //自动创建表，如果Entity有改到就更新到表结构
            Database.SetInitializer<EfDbContext>(new MigrateDatabaseToLatestVersion<EfDbContext, Migrations.Configuration>());
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new Configurations.CommentsEntityConfiguration());
            modelBuilder.Configurations.Add(new Configurations.CouponCostEntityConfiguration());
            modelBuilder.Configurations.Add(new Configurations.CouponSendLogEntityConfiguration());
            modelBuilder.Configurations.Add(new Configurations.GoodsEntityConfiguration());
            modelBuilder.Configurations.Add(new Configurations.NotifiesEntityConfiguration());
            modelBuilder.Configurations.Add(new Configurations.OrdersEntityConfiguration());
            modelBuilder.Configurations.Add(new Configurations.ProductsEntityConfiguration());
            modelBuilder.Configurations.Add(new Configurations.SchoolEntityConfiguration());
            modelBuilder.Configurations.Add(new Configurations.UserRoleEntityConfiguration());
            modelBuilder.Configurations.Add(new Configurations.UserRoleMapEntityConfiguration());
            modelBuilder.Configurations.Add(new Configurations.UsersEntityConfiguration());
            modelBuilder.Configurations.Add(new Configurations.SupplierEntityConfiguration());
            modelBuilder.Configurations.Add(new Configurations.UserRoleMapRegionEntityConfiguration());

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();     //不创建EdmMetadata表  //防止黑幕交易 要不然每次都要访问 EdmMetadata这个表
        }

     
        #region 表配置

        //public DbSet<AdvertEntity> AdvertEntities { get; set; }

        public DbSet<BrandsEntity> BrandsEntities { get; set; }

        public DbSet<CategoriesEntity> CategoriesEntities { get; set; }

        public DbSet<CommentsEntity> CommentsEntities { get; set; }

        public DbSet<CommentsFollowEntity> CommentsFollowEntities { get; set; }

        public DbSet<CouponCostLogsEntity> CouponCostLogsEntities { get; set; }

        public DbSet<CouponSendLogsEntity> CouponSendLogsEntities { get; set; }

        public DbSet<CouponsEntity> CouponsEntities { get; set; }

        public DbSet<CreditGoodsEntity> CreaGoodsEntities { get; set; }

        public DbSet<GoodsEntity> GoodsEntities { get; set; }

        public DbSet<NotifiesEntity> NotifiesEntities { get; set; }

        public DbSet<OrdersEntity> OrdersEntities { get; set; }

        public DbSet<ProductsEntity> ProductsEntities { get; set; }

        public DbSet<RegionEntity> RegionEntities { get; set; }

        public DbSet<SchoolEntity> SchoolEntities { get; set; }

        public DbSet<UserAddressEntity> UserAddressEntities { get; set; }

        public DbSet<CreditsExchangeLogsEntity> UserCreditsExchangeLogsEntities { get; set; }

        public DbSet<UserRoleEntity> UserRoleEntities { get; set; }

        public DbSet<UserRoleMapEntity> UserRoleMapEntities { get; set; }

        public DbSet<UsersEntity> UserEntities { get; set; }

        public DbSet<UserTradeLogsEntity> UserTradeLogsEntities { get; set; }

        public DbSet<SuppliersEntity> SuppliersEntities { get; set; }

        public DbSet<ProductsApplyEntity> ProductsApplyEntities { get; set; }

        public DbSet<GoodsOpreationLogsEntity> GoodsOpreationLogs { get; set; }

        public DbSet<UserRoleMapRegionEntity> UserRoleMapRegion { get; set; }


        #endregion

    }
}
