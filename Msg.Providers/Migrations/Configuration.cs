using System;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
using System.Linq;
using Msg.Entities;
using Msg.Providers.Repository;
using Msg.Utils;

namespace Msg.Providers.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Msg.Providers.Context.EfDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;//任何Model Class的修改將會直接更新DB
            AutomaticMigrationDataLossAllowed = true;

        }

        /// <summary>
        /// Seeds the specified data.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void Seed(Msg.Providers.Context.EfDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //添加默认品类
            var cateRep = EfRepository<CategoriesEntity, int>.Instance;
            if (!cateRep.LoadEntities().Any())
            {
                cateRep.AddEntity(new CategoriesEntity()
                {
                    Name = "牛奶"
                });
                cateRep.AddEntity(new CategoriesEntity()
                {
                    Name = "果干坚果"
                });
                cateRep.AddEntity(new CategoriesEntity()
                {
                    Name = "方便食品"
                });
                cateRep.AddEntity(new CategoriesEntity()
                {
                    Name = "冲泡饮料"
                });
                cateRep.AddEntity(new CategoriesEntity()
                {
                    Name = "休闲食品"
                });
                cateRep.AddEntity(new CategoriesEntity()
                {
                    Name = "地域特产"
                });
                cateRep.AddEntity(new CategoriesEntity()
                {
                    Name = "小美旅游"
                });
                cateRep.AddEntity(new CategoriesEntity()
                {
                    Name = "小美票务"
                });
                cateRep.AddEntity(new CategoriesEntity()
                {
                    Name = "学子用品"
                });
                cateRep.AddEntity(new CategoriesEntity()
                {
                    Name = "小美教育"
                });
                cateRep.AddEntity(new CategoriesEntity()
                {
                    Name = "日化用品",
                    IsForMarket = true
                });
                cateRep.AddEntity(new CategoriesEntity()
                {
                    Name = "护肤产品",
                    IsForMarket = true
                });
                cateRep.AddEntity(new CategoriesEntity()
                {
                    Name = "综合产品",
                    IsForMarket = true
                });
            }



            //添加默认区域
            var region = new RegionEntity();

            var repRegion = EfRepository<RegionEntity, int>.Instance;
            if (!repRegion.LoadEntities().Any())
            {
                region = repRegion.AddEntity(new RegionEntity()
                {
                    Name = "大学城",
                    PinyinCode = "daxuecheng",
                    Schools = new Collection<SchoolEntity>() { new SchoolEntity() { Name = "重庆大学", SchoolFirstCode = "C" } }
                });
            }


            //添加管理员用户
            var userRep = EfRepository<UsersEntity, int>.Instance;
            if (!userRep.LoadEntities(f => f.Email == "zhangke@meisugou.com").Any())
            {
                var salt = Utils.Utils.CreateVerifyCode(8);

                var user = new UsersEntity();

                user.Email = "zhangke@meisugou.com";
                user.NickName = "管理员";
                user.Password = Utils.Cryptography.Crypto.MD5("zhangkeAdmin" + salt);
                user.Salt = salt;
                user.UserName = "张科科";
                user.IsActive = true;
                var school = EfRepository<SchoolEntity, int>.Instance;

                user.School = school.FindById(1);

                user = userRep.AddEntity(user);

                var role =
                    EfRepository<UserRoleEntity, int>.Instance.AddEntity(new UserRoleEntity()
                    {
                        RoleName = "管理员",
                        IsUseable = true
                    });


                var roleMap = new UserRoleMapEntity() { CreateTime = DateTime.Now, Role = role, User = user };

                EfRepository<UserRoleMapEntity, int>.Instance.AddEntity(roleMap);

                user.RoleMap = roleMap;
                userRep.UpdateEntity(user);
            }


            //添加优惠券
            var couponRep = EfRepository<CouponsEntity, Guid>.Instance;
            if (!couponRep.LoadEntities().Any())
            {
                var coupon = new CouponsEntity()
                {
                    CouponName = "注册赠送-满50省5元抵扣优惠券",
                    Logo = "null",
                    CouponValue = 5,
                    FullPrice = 50,
                    ReducePrice = 5,
                    Type = CouponTypeEnum.FullReduceDeduction,
                    IsUseable = true,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Parse(ConfigHelper.GetConfigString("RegistSendCouponEndTime"))

                };
                couponRep.AddEntity(coupon);
            }


        }
    }
}
