using System.Collections.Generic;
using System.Text;
using Msg.Entities;
using Msg.Mail;
using Msg.Utils.Cryptography;

namespace Msg.Web.Models
{
    public class UserMail
    {
        /// <summary>
        /// 注册发送邮件
        /// </summary>
        /// <param name="uRealName"></param>
        /// <param name="activeUrl"></param>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        public static bool RegisterMail(UsersEntity u, string return_url)
        {
            if (u == null) return false;

            string VerifyData = "uMail=" + u.Email.ToLower() + "&Salt=" + u.Salt;
            VerifyData = Crypto.MD5(VerifyData);
            string activeUrl = "";
            if (!string.IsNullOrWhiteSpace(return_url))
            {
                activeUrl = "http://www.meisugou.com/account/ActiveOk?Gid=" + u.Id + "&VerifyData=" + VerifyData + "&return_url=" + return_url;
            }
            else
            {
                activeUrl = "http://www.meisugou.com/account/ActiveOk?Gid=" + u.Id + "&VerifyData=" + VerifyData;
            }
            List<string> RecipientEmails = new List<string>();
            RecipientEmails.Add(u.Email);

            SmtpMailBase smail = new SmtpMailBase();
            smail.Html = true;
            smail.Subject = "已成功创建您的美速购帐户,请查收";
            smail.RecipientEmails = RecipientEmails;
            StringBuilder body = new StringBuilder();
            body.Append("亲爱的用户 " + u.NickName + "：<br />欢迎加入美速购<br />请点击以下链接即可完成注册<br /><br />");
            body.Append("<a href=\"" + activeUrl + "\">" + activeUrl + "</a>");
            body.Append("<br />(如果以上链接无法点击，请将上面的地址复制到你的浏览器（如IE）以完成激活)<br /><br />");
            body.Append("--美速购<br />（感谢登录美速购，这是一封自动产生的email，请勿回复）");
            smail.Body = body.ToString();
            return smail.Send();


        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        internal static bool PassMail(UsersEntity u)
        {
            if (u == null) return false;

            string VerifyData = "uMail=" + u.Email.ToLower() + "&Salt=" + u.Salt;
            VerifyData = Crypto.MD5(VerifyData);
            string activeUrl = "http://www.meisugou.com/account/resetpwd?Gid=" + u.Id + "&VerifyData=" + VerifyData;
            List<string> RecipientEmails = new List<string>();
            RecipientEmails.Add(u.Email);

            SmtpMailBase smail = new SmtpMailBase();
            smail.Html = true;
            smail.Subject = "重设您美速购帐户密码,请查收";
            smail.RecipientEmails = RecipientEmails;
            StringBuilder body = new StringBuilder();
            body.Append("亲爱的用户 " + u.NickName + "：<br />请点击以下链接即可重设帐户密码<br /><br />");
            body.Append("<a href=\"" + activeUrl + "\">" + activeUrl + "</a>");
            body.Append("<br />(如果以上链接无法点击，请将上面的地址复制到你的浏览器（如IE）以完成密码重设)<br /><br />");
            body.Append("--美速购<br />（感谢登录美速购，这是一封自动产生的email，请勿回复）");
            smail.Body = body.ToString();
            return smail.Send();
        }

        /// <summary>
        /// 发送邀请邮件
        /// </summary>
        /// <param name="emails"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool SendInviteMail(List<string> emails, string title, string content, string inviteurl)
        {
            SmtpMailBase smail = new SmtpMailBase();
            smail.Html = true;
            smail.Subject = title;
            smail.RecipientEmails = emails;
            StringBuilder body = new StringBuilder();
            body.Append(content);
            body.Append("<br/><br/>");
            body.Append("<p>-----------------------------------------------------------------------------------------</p>");
            body.AppendFormat("<p>请点击一下链接完成注册：<a href=\"{0}\">{0}</a></p>", inviteurl);

            smail.Body = body.ToString();
            return smail.Send();
        }

    }
}
