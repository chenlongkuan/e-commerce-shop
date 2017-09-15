using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Msg.Bll.Adapter;
using Msg.Bll.Models;
using Msg.Entities;
using Msg.Providers.Repository;
using Msg.Redis;
using Msg.Tools;
using Msg.Tools.ExpressionExtend;
using Msg.Tools.Logging;
using Msg.Utils;
using Msg.Utils.Cryptography;
using CacheHelper = Msg.Redis.CacheHelper;

namespace Msg.Bll.Helpers
{
    /// <summary>
    /// 用户帮助类
    /// </summary>
    public class UsersHelper
    {
        private readonly EfRepository<UsersEntity, int> _userRepository = EfRepository<UsersEntity, int>.Instance;
        private readonly EfRepository<UserRoleEntity, int> _roleRepository = EfRepository<UserRoleEntity, int>.Instance;
        private readonly EfRepository<SchoolEntity, int> _schoolRepository = EfRepository<SchoolEntity, int>.Instance;

        private readonly EfRepository<UserAddressEntity, int> _addressRepository =
            EfRepository<UserAddressEntity, int>.Instance;

        #region 单例

        private static UsersHelper _instance;

        public static UsersHelper Instance
        {
            get { return _instance ?? (_instance = new UsersHelper()); }
        }

        #endregion



        /// <summary>
        /// 从缓存中获取用户实体
        /// </summary>
        /// <param name="uid">用户Id</param>
        /// <returns></returns>
        public UserModel GetUserCacheModel(int uid)
        {
            var cacheObj = CacheHelper.Get<UserModel>(CacheKeys.USER);
            if (cacheObj != null) return cacheObj;

            var user = _userRepository.FindById(uid);
            if (user != null)
            {
                cacheObj = user.ProjectedAs<UserModel, int>();
                cacheObj.SchoolId = user.School.Id;
            }
            if (cacheObj != null)
            {
                CacheHelper.Put(CacheKeys.USER + uid.ToString(), cacheObj, CacheTimeOut.UserModel);
            }
            return cacheObj;
        }

        /// <summary>
        /// Gets the user count.
        /// </summary>
        /// <returns></returns>
        public int GetUserCount()
        {
            return _userRepository.LoadEntities().Count();
        }



        /// <summary>
        /// Regists the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        public OperationResult Regist(UsersEntity user, int schoolId)
        {
            var status = false;
            PublicHelper.CheckArgument(user, "user");


            try
            {
                if (schoolId <= 0)
                {
                    return new OperationResult(OperationResultType.ParamError, "学校未选择");
                }

                //登录名格式
                if (!Utils.Utils.IsValidEmail(user.Email))
                {
                    return new OperationResult(OperationResultType.ParamError, "登陆名邮箱格式错误");
                }
                if (string.IsNullOrEmpty(user.Password))
                {
                    return new OperationResult(OperationResultType.ParamError, "密码不可为空");
                }

                if (user.Password.Length < 8 || user.Password.Length > 16)
                {
                    return new OperationResult(OperationResultType.ParamError, "请填写8-16位的密码");
                }
                if (string.IsNullOrEmpty(user.NickName))
                {
                    return new OperationResult(OperationResultType.ParamError, "昵称不可为空");
                }
                if (user.NickName.Length > 20)
                {
                    return new OperationResult(OperationResultType.ParamError, "昵称超出长度限制");
                }

                var isNickNameExits = IsNickNameExits(user.NickName);

                if (isNickNameExits)
                {
                    return new OperationResult(OperationResultType.ParamError, "昵称已经存在");
                }


                if (IsEmailExits(user.Email))
                {
                    return new OperationResult(OperationResultType.ParamError, "注册邮箱已经存在");
                }

                user.School = _schoolRepository.FindById(schoolId);
                if (user.School == null)
                {
                    return new OperationResult(OperationResultType.QueryNull, "学校不存在");
                }

                user.Password = user.Password.ToLower();
                user.Salt = Utils.Utils.CreateVerifyCode(8);
                user.Password = Crypto.MD5(user.Password + user.Salt);
                user.IsActive = true;//默认激活
                user = _userRepository.AddEntity(user);


                if (user.Id > 0)
                {
                    //赠送优惠券
                    if (DateTime.Now < DateTime.Parse(ConfigHelper.GetConfigString("RegistSendCouponEndTime")))
                    {
                        CouponsHelper.Instance.SendCouponForRegist(user);
                    }

                    ////异步
                    //Task.Factory.StartNew(() =>
                    //{  
                    //});

                    //颁发票证
                    OpenTicket(user.Id, user.NickName, user.Email, true);
                    //登陆动作
                    LoginAction(user);
                    return new OperationResult(OperationResultType.Success, "注册成功", user);
                }

                return new OperationResult(OperationResultType.Error, "注册失败");
            }
            catch (Exception ex)
            {
                LogHelper.WriteException("注册失败，发生异常", ex);
                return new OperationResult(OperationResultType.Error, "注册失败，发生异常");
            }

        }



        /// <summary>
        /// Logins the specified login.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <returns></returns>
        public OperationResult Login(LoginModel login)
        {
            if (string.IsNullOrEmpty(login.Account) || string.IsNullOrEmpty(login.Password))
            {
                return new OperationResult(OperationResultType.ParamError, "账户名或密码为空");
            }


            var user = _userRepository.LoadEntities(u => u.Email == login.Account).SingleOrDefault();
            if (user == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "用户不存在");
            }

            var md5Pwd = Crypto.MD5(login.Password + user.Salt);
            if (user.Password == md5Pwd)
            {
                //颁发票证
                OpenTicket(user.Id, user.NickName, user.Email, login.IsRememberLogin);
                //登陆动作
                LoginAction(user);
                return new OperationResult(OperationResultType.Success, "认证成功", user);
            }

            return new OperationResult(OperationResultType.PurviewLack, "密码错误");

        }

        /// <summary>
        /// Quicks the login validata user.
        /// </summary>
        /// <param name="connactUserIdentity">The connact user identity.</param>
        /// <param name="connactUserFrom">The connact user from.</param>
        /// <returns></returns>
        public OperationResult QuickLoginValidataUser(string connactUserIdentity, string connactUserFrom)
        {
            var user = GetUserByConnactInfo(connactUserIdentity, connactUserFrom);

            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.Email))
                {
                    return new OperationResult(OperationResultType.IllegalOperation, "用户邮箱不存在");
                }

                if (!user.IsUseable)
                {
                    return new OperationResult(OperationResultType.IllegalOperation, "用户不可用");
                }

                //颁发票证
                OpenTicket(user.Id, user.NickName, user.Email, true);
                //登陆动作
                LoginAction(user);
                return new OperationResult(OperationResultType.Success, "认证成功", user);
            }

            return new OperationResult(OperationResultType.QueryNull, "不存在该用户");

        }





        /// <summary>
        /// Determines whether [is nick name exits] [the specified user].
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        public bool IsNickNameExits(string nickName)
        {

            var isNickNameExits = _userRepository.LoadEntities(u => u.NickName == nickName).Any();

            return isNickNameExits;
        }

        /// <summary>
        /// Determines whether [is email exits] [the specified email].
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public bool IsEmailExits(string email)
        {

            var isNickNameExits = _userRepository.LoadEntities(u => u.Email == email).Any();

            return isNickNameExits;
        }


        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public UsersEntity GetUser(int userId)
        {
            var user = _userRepository.FindById(userId);
            return user;
        }


        /// <summary>
        /// Gets the user by email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public UsersEntity GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }
            var query = _userRepository.LoadEntities(f => f.Email == email).SingleOrDefault();
            return query;
        }


        /// <summary>
        /// Gets the users entities.
        /// </summary>
        /// <param name="schoolId">The school identifier.</param>
        /// <param name="nickName">Name of the nick.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="page">The page.</param>
        /// <param name="size">The size.</param>
        /// <param name="total">The total.</param>
        /// <returns></returns>
        public IQueryable<UsersEntity> GetUsersEntities(int? schoolId, string nickName, string sortColumn, int page,
            int size, out int total)
        {
            Expression<Func<UsersEntity, bool>> filter = f => (f.RoleMap.Role.RoleName ?? "") != "管理员";
            Expression<Func<UsersEntity, int>> sortCondition;
            if (schoolId.HasValue)
            {
                filter = f => f.School.Id == schoolId.Value;
            }
            if (!string.IsNullOrEmpty(nickName))
            {
                filter = filter.And(f => f.NickName.Contains(nickName));
            }
            switch (sortColumn)
            {
                case "credit":
                    sortCondition = s => s.Credits;
                    break;
                default:
                    sortCondition = s => s.Id;
                    break;
            }

            var query = _userRepository.LoadEntitiesByPaging(page, size, filter, sortCondition, OrderingOrders.DESC,
                out total);

            return query;

        }

        /// <summary>
        ///修改头像
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="avatar">The avatar.</param>
        /// <returns></returns>
        public bool ModifyAvatar(int userId, string avatar)
        {
            if (!string.IsNullOrEmpty(avatar) && userId > 0)
            {
                var user = GetUser(userId);
                if (user != null)
                {
                    user.Avatar = avatar;
                    var status = _userRepository.UpdateEntity(user);
                    return status;
                }
            }
            return false;
        }

        /// <summary>
        /// Modifies the password.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="oldPwd">The old password.</param>
        /// <param name="newPwd">The new password.</param>
        /// <returns></returns>
        public OperationResult ModifyPwd(int userId, string oldPwd, string newPwd)
        {
            if (string.IsNullOrEmpty(oldPwd))
            {
                return new OperationResult(OperationResultType.ParamError, "原密码");
            }
            if (string.IsNullOrEmpty(newPwd))
            {
                return new OperationResult(OperationResultType.ParamError, "新密码");
            }
            if (newPwd.Length < 8 || newPwd.Length > 16)
            {
                return new OperationResult(OperationResultType.ParamError, "新密码长度不正确，应在8-16字符以内");
            }


            var user = GetUser(userId);
            if (user == null || !user.IsUseable)
            {
                return new OperationResult(OperationResultType.QueryNull, "用户不存在或不可用");
            }

            var md5Pwd = Crypto.MD5(oldPwd + user.Salt);
            if (md5Pwd != user.Password)
            {
                return new OperationResult(OperationResultType.PurviewLack, "原密码错误");
            }
            user.Password = Crypto.MD5(newPwd + user.Salt);

            var status = _userRepository.UpdateEntity(user);

            return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="verifyStr">The verify string.</param>
        /// <param name="newPwd">The new password.</param>
        /// <returns></returns>
        public OperationResult ResetPwd(int userId, string verifyStr, string newPwd)
        {
            if (userId <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "Gid");
            }
            if (string.IsNullOrEmpty(verifyStr))
            {
                return new OperationResult(OperationResultType.ParamError, "验证字符串");
            }
            if (string.IsNullOrEmpty(newPwd))
            {
                return new OperationResult(OperationResultType.ParamError, "新密码");
            }
            if (newPwd.Length < 8 || newPwd.Length > 16)
            {
                return new OperationResult(OperationResultType.ParamError, "新密码长度不正确，应在8-16字符以内");
            }

            UsersEntity u = GetUser(userId);
            string DataLocal = "uMail=" + u.Email.ToLower() + "&Salt=" + u.Salt;
            string toVerifyData = Crypto.MD5(DataLocal);
            if (toVerifyData.Equals(verifyStr))
            {
                newPwd = newPwd.ToLower();
                string salt = Utils.Utils.CreateVerifyCode(8);
                string newPassword = Crypto.MD5(newPwd + salt);
                u.Password = newPassword;
                u.Salt = salt;
                var status = _userRepository.UpdateEntity(u);

                return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged);
            }
            return new OperationResult(OperationResultType.IllegalOperation, "验证未通过");
        }


        /// <summary>
        /// Toogles the useable.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public OperationResult ToogleUseable(int userId)
        {
            if (userId <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "userId");
            }
            var user = GetUser(userId);
            if (user == null) return new OperationResult(OperationResultType.QueryNull, "user");
            user.IsUseable = !user.IsUseable;
            var status = _userRepository.UpdateEntity(user);
            if (status)
            {
                //更新缓存
                CacheUtils.UpdateCache(CacheKeys.USER, CacheTimeOut.UserModel, user.ProjectedAs<UserModel, int>());
                return new OperationResult(OperationResultType.Success);
            }
            return new OperationResult(OperationResultType.NoChanged);
        }


        /// <summary>
        /// 激活用户
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public OperationResult ActiveUser(int userId)
        {
            if (userId <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "userId");
            }
            var user = GetUser(userId);
            if (user == null) return new OperationResult(OperationResultType.QueryNull, "user");

            if (user.IsActive)
            {
                return new OperationResult(OperationResultType.Success);
            }

            user.IsActive = true;
            var status = _userRepository.UpdateEntity(user);
            if (status)
            {
                //颁发票证
                OpenTicket(user.Id, user.NickName, user.Email, true);
                //登陆动作
                LoginAction(user);

                //更新缓存
                CacheUtils.UpdateCache(CacheKeys.USER, CacheTimeOut.UserModel, user.ProjectedAs<UserModel, int>());

                return new OperationResult(OperationResultType.Success, "认证成功", user);
            }
            return new OperationResult(OperationResultType.NoChanged);
        }


        /// <summary>
        /// 登陆动作
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        public void LoginAction(int userId)
        {
            var user = GetUser(userId);
            if (user != null)
            {
                LoginAction(user);
            }
        }

        /// <summary>
        /// Modifies the user credit.
        /// </summary>
        /// <param name="payPrice">The pay price.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public OperationResult ModifyUserCredit(decimal payPrice, int userId)
        {
            var user = GetUser(userId);
            if (user != null)
            {
                user.Credits = (int)(payPrice / 1);
                var status = _userRepository.UpdateEntity(user);
                return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged);
            }
            return new OperationResult(OperationResultType.QueryNull, "用户不存在");
        }


        /// <summary>
        /// Gets the user role.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public UserRoleMapEntity GetUserRole(int userId)
        {
            if (userId < 0)
            {
                return null;
            }
            return _userRepository.LoadEntities(f => f.Id == userId).FirstOrDefault().RoleMap;

        }

        /// <summary>
        /// 从缓存中获取用户角色
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public UserRoleModel GetUserRoleByCache(int userId)
        {
            var cacheObj = CacheHelper.Get<UserRoleModel>(CacheKeys.ROLE);
            if (cacheObj != null) return cacheObj;

            var roleMap = GetUserRole(userId);
            if (roleMap != null)
            {
                cacheObj = new UserRoleModel()
                {
                    IsUseable = roleMap.Role.IsUseable,
                    RoleName = roleMap.Role.RoleName
                };
            }

            if (cacheObj != null)
            {
                CacheHelper.Put(CacheKeys.ROLE + userId.ToString(), cacheObj, CacheTimeOut.UserRole);
            }
            return cacheObj;
        }


        #region 用户收货地址

        /// <summary>
        /// Gets the user school region.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public int GetUserSchoolRegion(int userId)
        {
            var cacheObj = CacheHelper.Get<int>(CacheKeys.USER_REGION);
            if (cacheObj > 0) return cacheObj;

            cacheObj = GetUser(userId).School.Region.Id;

            if (cacheObj > 0)
            {
                CacheHelper.Put(CacheKeys.USER_REGION + userId.ToString(), cacheObj);
            }
            return cacheObj;
        }

        /// <summary>
        /// Gets the user address entity.
        /// </summary>
        /// <param name="addressId">The address identifier.</param>
        /// <returns></returns>
        public UserAddressEntity GetUserAddressEntity(int addressId)
        {
            return _addressRepository.FindById(addressId);
        }


        /// <summary>
        /// Gets the user address entities.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IQueryable<UserAddressEntity> GetUserAddressEntities(int userId)
        {
            return _addressRepository.LoadEntities(f => f.User.Id == userId && f.IsUseable);
        }

        /// <summary>
        /// 根据用户和地址id查询地址信息
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public UserAddressEntity GetUserDefaultAddressEntities(int userId, int addressId)
        {
            UserAddressEntity address = null;
            if (addressId > 0)
            {
                address = _addressRepository.LoadEntities(r => r.Id == addressId && r.User.Id == userId).FirstOrDefault();
            }
            if (address == null)
            {
                var list = _addressRepository.LoadEntities(f => f.User.Id == userId && f.IsUseable);
                address = list.SingleOrDefault(f => f.IsDefult) ?? list.FirstOrDefault();
            }
            return address;
        }

        /// <summary>
        /// Modifies the address.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public OperationResult SaveAddress(int userId, UserAddressEntity address)
        {
            if (address.Id < 0)
            {
                return new OperationResult(OperationResultType.ParamError, "Id");
            }
            if (string.IsNullOrEmpty(address.ReciverName))
            {
                return new OperationResult(OperationResultType.ParamError, "收货人");
            }
            if (string.IsNullOrEmpty(address.ReciverTel))
            {
                return new OperationResult(OperationResultType.ParamError, "联系电话");
            }
            if (string.IsNullOrEmpty(address.DetailAddress))
            {
                return new OperationResult(OperationResultType.ParamError, "收货地址");
            }
            if (address.SchoolId <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "请选择学校");
            }


            if (address.Id > 0)//编辑
            {
                var entity = _addressRepository.FindById(address.Id);
                if (entity == null || !entity.IsUseable)
                {
                    return new OperationResult(OperationResultType.QueryNull, "查无此记录");
                }
                if (entity.User.Id != userId)
                {
                    return new OperationResult(OperationResultType.PurviewLack, "此记录不属于你");
                }
                entity.ReciverName = address.ReciverName;
                entity.ReciverTel = address.ReciverTel;
                entity.PostCode = address.PostCode;
                entity.DetailAddress = address.DetailAddress;
                entity.SchoolId = address.SchoolId;
                entity.SchoolName = address.SchoolName;
                entity.RegionId = address.RegionId;
                entity.RegionName = address.RegionName;
                entity.CityName = address.CityName;
                var status = _addressRepository.UpdateEntity(entity);
                entity.User = null;
                return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged, "", entity);
            }
            else//新增
            {
                var user = _userRepository.LoadEntities(f => f.Id == userId && f.IsUseable, false).SingleOrDefault();
                if (user == null)
                {
                    return new OperationResult(OperationResultType.QueryNull, "用户不存在或被禁用");
                }
                if (!user.IsActive)
                {
                    return new OperationResult(OperationResultType.PurviewLack, "用户未激活");
                }
                address.User = user;
                address.IsUseable = true;
                address = _addressRepository.AddEntity(address);
                address.User = null;
                return new OperationResult(address.Id > 0 ? OperationResultType.Success : OperationResultType.NoChanged, "", address);
            }

        }

        /// <summary>
        /// Deletes the address.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public OperationResult DeleteAddress(int userId, int id)
        {
            if (id <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "Id");
            }
            var address = _addressRepository.FindById(id);
            if (address == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "查无此记录");
            }
            if (address.User.Id != userId)
            {
                return new OperationResult(OperationResultType.PurviewLack, "此记录不属于你");
            }
            address.IsUseable = false;
            address.IsDefult = false;

            var status = _addressRepository.UpdateEntity(address);

            return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged);
        }

        /// <summary>
        /// 设置默认收货地址
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public OperationResult SetToDefaultAddress(int userId, int id)
        {
            if (id <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "Id");
            }
            var address = _addressRepository.FindById(id);
            if (address == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "查无此记录");
            }
            if (address.User.Id != userId)
            {
                return new OperationResult(OperationResultType.PurviewLack, "此记录不属于你");
            }

            address.IsDefult = true;

            var status = _addressRepository.UpdateEntity(address);
            if (status)
            {
                var ta = Task.Factory.StartNew(() =>
                {
                    var sql = string.Format(@"UPDATE dbo.UserAddressEntity SET IsDefult=0 WHERE User_Id={0} AND Id<>{1}", userId, address.Id);
                    _addressRepository.ExecuteCommand(sql);
                });
            }

            return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged);
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// 登陆动作
        /// </summary>
        /// <param name="user">The user.</param>
        private void LoginAction(UsersEntity user)
        {

            user.LoginTimes += 1;
            user.LastLoginTime = user.LoginTime;
            user.LoginTime = DateTime.Now;

            _userRepository.UpdateEntity(user);

            //更新缓存
            CacheUtils.UpdateCache(CacheKeys.USER, CacheTimeOut.UserModel, user.ProjectedAs<UserModel, int>());

        }



        /// <summary>
        /// 颁发票证
        /// </summary>
        private void OpenTicket(int userId, string userName, string loginName, bool remberMe)
        {
            //用户登录loginName，用户id，用户名，票证版本 
            string ticketStr = loginName + "##" + userId + "##" + userName + "##2.0";
            FormsAuthentication.SetAuthCookie(HttpUtility.UrlEncode(ticketStr), remberMe);
            //更新用户缓存
            CacheHelper.Remove(CacheKeys.USER);
        }

        /// <summary>
        /// 验证系统是否存在链接用户记录
        /// </summary>
        /// <param name="connactUserIdentity">用户标识</param>
        /// <param name="connactUserFrom">用户来源名称</param>
        private UsersEntity GetUserByConnactInfo(string connactUserIdentity, string connactUserFrom)
        {
            if (string.IsNullOrEmpty(connactUserFrom) || string.IsNullOrEmpty(connactUserIdentity))
            {
                return null;
            }

            return
                _userRepository.LoadEntities(
                    f => f.ConnactUserFrom == connactUserFrom && f.ConnactUserIdentity == connactUserIdentity)
                    .SingleOrDefault();

        }



        #endregion


    }
}
