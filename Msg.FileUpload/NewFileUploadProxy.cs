using System.ServiceModel;
using WCF.Lib.File;

namespace Msg.FileUpload
{
    class NewFileUploadProxy : ServiceProxy<INewFileUpload>
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="binding">MessageEncoding=Mtom</param>
        /// <param name="remoteAddress"></param>
        internal NewFileUploadProxy(WSHttpBinding binding, string remoteAddress) : base(binding, remoteAddress) { }
    }
}
