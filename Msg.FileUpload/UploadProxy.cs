using System.ServiceModel;
using WCF.Lib.File;

namespace Msg.FileUpload
{
    class UploadProxy : ServiceProxy<IUpload>
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="remoteAddress"></param>
        internal UploadProxy(string remoteAddress) : base(new WSHttpBinding(SecurityMode.None), remoteAddress)
        {

        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="binding">MessageEncoding=Mtom</param>
        /// <param name="remoteAddress"></param>
        internal UploadProxy(WSHttpBinding binding, string remoteAddress) : base(binding, remoteAddress)
        {

        }
    }
}
