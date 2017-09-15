

using System;

namespace Msg.Web.Models
{
    #region QQ

    public class QQ_ACCESS_TOKEN
    {
        /// <summary>
        /// 准许令牌
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 过期时间  秒
        /// </summary>
        public string expires_in { get; set; }
    }


    /// <summary>
    /// QQ开放ID实体
    /// </summary>
    public class QQ_LOGIN_OPENID_MODEL
    {
        /// <summary>
        /// APP ID
        /// </summary>
        public int? client_id { get; set; }
        /// <summary>
        /// 用户的ID，与QQ号码一一对应。
        /// </summary>
        public string openid { get; set; }

        /// <summary>
        /// 错误代码  成功时不会返回值
        /// </summary>
        public int? error { get; set; }

        /// <summary>
        /// 错误描述  成功时不会返回值
        /// </summary>
        public string error_description { get; set; }

    }

    /// <summary>
    /// QQ用户信息实体
    /// </summary>
    public class QQ_LOGIN_USERINFO_MODEL
    {

        /// <summary>
        /// 返回状态码
        /// </summary>
        public int ret { get; set; }

        /// <summary>
        /// 如果ret小于0，会有相应的错误信息提示
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string nickname { get; set; }

        /// <summary>
        /// 头像 30*30
        /// </summary>
        public string figureurl { get; set; }

        /// <summary>
        /// 头像 50*50
        /// </summary>
        public string figureurl_1 { get; set; }

        /// <summary>
        /// 头像 100*100
        /// </summary>
        public string figureurl_2 { get; set; }

        /// <summary>
        /// 性别。如果获取不到则默认返回“男”
        /// </summary>
        public string gender { get; set; }

        /// <summary>
        /// 标识用户是否为黄钻用户（0：不是；1：是）
        /// </summary>
        public int vip { get; set; }

        /// <summary>
        /// 黄钻等级
        /// </summary>
        public int level { get; set; }

        /// <summary>
        /// 标识是否为年费黄钻用户（0：不是； 1：是）
        /// </summary>
        public int is_yellow_year_vip { get; set; }


    }
    #endregion

    #region SINA

    public class SINA_LOGIN_UID
    {
        /// <summary>
        /// 用户标识 UID
        /// </summary>
        public string uid { get; set; }

        /// <summary>
        ///请求接口地址
        /// </summary>
        public string request { get; set; }

        /// <summary>
        /// 错误代码   查询地址   http://open.weibo.com/wiki/Error_code
        /// </summary>
        public string error_code { get; set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        public string error { get; set; }
    }


    public class SINA_LOGIN_USERSHOW
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Int64 id { get; set; }

        /// <summary>
        /// 字符串型UID
        /// </summary>
        public string idStr { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string screen_name { get; set; }

        /// <summary>
        /// 友好显示名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 用户所在省级ID
        /// </summary>
        public int province { get; set; }

        /// <summary>
        /// 用户所在城市ID
        /// </summary>
        public int city { get; set; }

        /// <summary>
        /// 用户所在地
        /// </summary>
        public string location { get; set; }

        /// <summary>
        /// 用户个人描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 用户博客地址
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 用户头像地址，50×50像素
        /// </summary>
        public string profile_image_url { get; set; }


        /// <summary>
        /// 用户的微博统一URL地址
        /// </summary>
        public string profile_url { get; set; }

        /// <summary>
        /// 用户的个性化域名
        /// </summary>
        public string domain { get; set; }


        /// <summary>
        /// 用户的微号
        /// </summary>
        public string weihao { get; set; }

        /// <summary>
        /// 性别，m：男、f：女、n：未知
        /// </summary>
        public string gender { get; set; }

        /// <summary>
        /// 粉丝数
        /// </summary>
        public int followers_count { get; set; }

        /// <summary>
        /// 关注数
        /// </summary>
        public int friends_count { get; set; }

        /// <summary>
        /// 微博数
        /// </summary>
        public int statuses_count { get; set; }

        /// <summary>
        /// 收藏数
        /// </summary>
        public int favourites_count { get; set; }

        /// <summary>
        /// 用户创建（注册）时间
        /// </summary>
        public string created_at { get; set; }

        /// <summary>
        /// 暂未支持
        /// </summary>
        public bool following { get; set; }

        /// <summary>
        /// 是否允许所有人给我发私信，true：是，false：否
        /// </summary>
        public bool allow_all_act_msg { get; set; }

        /// <summary>
        /// 是否允许标识用户的地理位置，true：是，false：否
        /// </summary>
        public bool geo_enabled { get; set; }

        /// <summary>
        /// 是否是微博认证用户，即加V用户，true：是，false：否
        /// </summary>
        public bool verified { get; set; }

        /// <summary>
        /// 暂未支持
        /// </summary>
        public int verified_type { get; set; }

        /// <summary>
        /// 用户备注信息，只有在查询用户关系时才返回此字段
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 用户的最近一条微博信息字段  目前暂不扩展  后期需要在
        ///  http://open.weibo.com/wiki/%E5%B8%B8%E8%A7%81%E8%BF%94%E5%9B%9E%E5%AF%B9%E8%B1%A1%E6%95%B0%E6%8D%AE%E7%BB%93%E6%9E%84#.E5.BE.AE.E5.8D.9A.EF.BC.88status.EF.BC.89
        /// 查询对象数据结构
        /// </summary>
        public object status { get; set; }

        /// <summary>
        /// 是否允许所有人对我的微博进行评论，true：是，false：否
        /// </summary>
        public bool allow_all_comment { get; set; }

        /// <summary>
        /// 用户大头像地址
        /// </summary>
        public string avatar_large { get; set; }

        /// <summary>
        /// 认证原因
        /// </summary>
        public string verified_reason { get; set; }

        /// <summary>
        /// 该用户是否关注当前登录用户，true：是，false：否
        /// </summary>
        public bool follow_me { get; set; }

        /// <summary>
        /// 用户的在线状态，0：不在线、1：在线
        /// </summary>
        public int online_status { get; set; }

        /// <summary>
        /// 用户的互粉数
        /// </summary>
        public int bi_followers_count { get; set; }

        /// <summary>
        /// 用户当前的语言版本，zh-cn：简体中文，zh-tw：繁体中文，en：英语
        /// </summary>
        public string lang { get; set; }

        /// <summary>
        ///请求接口地址
        /// </summary>
        public string request { get; set; }

        /// <summary>
        /// 错误代码   查询地址   http://open.weibo.com/wiki/Error_code
        /// </summary>
        public string error_code { get; set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        public string error { get; set; }
    }


    #endregion

    #region 保存完善信息实体

    public class SaveContactModel
    {
        /// <summary>
        /// 用户标识
        /// </summary>
        public string UserIdentity { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string uNickName { get; set; }

        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string uMail { get; set; }

        /// <summary>
        /// 用户性别
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// 学校ID
        /// </summary>
        public int SchoolId { get; set; }

        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 保存ACCESSTOKEN
        /// </summary>
        public string SaveToken { get; set; }

        /// <summary>
        /// 城市ID
        /// </summary>
        public int ProvinceId { get; set; }
    }

    #endregion
}