using System.Collections.Generic;
using System.Text;
using Msg.Config;
using Msg.Tools.Logging;

namespace Msg.Mail
{
    /// <summary>
    /// SMTP邮件
    /// </summary>
    public class SmtpMailBase
    {
        #region 公有属性

        /// <summary>
        /// 发件人姓名
        /// </summary>
        private string FromName = "美速购";

        /// <summary>
        /// 邮件主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 邮件正文
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        ///  是否Html邮件
        /// </summary>
        public bool Html { get; set; }

        /// <summary>
        /// 收件人的邮箱地址列表
        /// </summary>
        public List<string> RecipientEmails { get; set; }

        #endregion

        #region 公共方法

        /// <summary>
        /// 发送邮件
        /// </summary>
        public bool Send()
        {
            if (RecipientEmails.Count == 0 || string.IsNullOrEmpty(Body))
            {
                return false;
            }

            EmailConfigInfo config = EmailConfigManager.GetConfig(); // 获取邮件服务器 Config 信息
            Encoding eEncod = Encoding.UTF8;  // 邮件编码信息
            bool result = false;

            #region 初始化邮件服务器信息

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient()
            {
                Host = config.Smtp,
                Port = config.Port,
                EnableSsl = config.Port != 25,   //  当不是25端口(gmail:587) 时 启用 SSL 加密连接
                Credentials = new System.Net.NetworkCredential(config.Username, config.Password)
            };

            #endregion

            #region 初始化邮件信息

            /// 初始化邮件信息
            System.Net.Mail.MailMessage myEmail = new System.Net.Mail.MailMessage()
            {
                From = new System.Net.Mail.MailAddress(config.Sysemail, FromName, eEncod), // 发送地址
                SubjectEncoding = eEncod, /// 编码格式
                Subject = Subject,  /// 邮件主题
                Body = Body,    /// 邮件内容
                Priority = System.Net.Mail.MailPriority.Normal,
                IsBodyHtml = true,
            };

            /// 添加收件人
            foreach (string item in RecipientEmails)
            {
                myEmail.To.Add(item);
            }

            #endregion

            #region 发送邮件

            try
            {
                smtp.Send(myEmail);
                result = true;
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                string assembly = "";
                //取得当前方法命名空间
                assembly += "命名空间名:" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace + "\n";
                //取得当前方法类全名
                assembly += "类名:" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "\n";
                //取得当前方法名
                assembly += "方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n";
                LogHelper.WriteException(assembly, ex);

            }

            #endregion

            return result;
        }

        #endregion

    }
}