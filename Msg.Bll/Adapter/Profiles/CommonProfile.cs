using AutoMapper;
using Msg.Bll.Models;
using Msg.Entities;

namespace Msg.Bll.Adapter.Profiles
{
    /// <summary>
    /// 对象转换配置
    /// </summary>
    public class CommonProfile : Profile
    {
        protected override void Configure()
        {
           
            //UsersEntity Mapping
            var userMapping = Mapper.CreateMap<UsersEntity, UserModel>();

            //BrandsEntity Mapping
            var brandsMapping = Mapper.CreateMap<BrandsEntity, BrandsModel>();

            var cateMapping = Mapper.CreateMap<CategoriesEntity, CategoryModel>();


            var roleMapMapping = Mapper.CreateMap<UserRoleMapEntity, UserRoleModel>();
            roleMapMapping.ForMember(f => f.RoleName, m => m.MapFrom(r => r.Role.RoleName));
        }


    }
}
