using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    /// 权限对应区域映射表
    /// </summary>
    public class UserRoleMapRegionEntity : BaseEntity<int>
    {
        public UserRoleMapEntity RoleMap { get; set; }

        public RegionEntity Region { get; set; }
    }
}
