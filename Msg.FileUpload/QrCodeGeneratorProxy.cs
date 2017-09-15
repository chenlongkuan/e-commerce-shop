using System.ServiceModel.Channels;
using WCF.WEB.FileService;

namespace Msg.FileUpload
{
    class QrCodeGeneratorProxy : ServiceProxy<IQrCodeGenerator>
    {
        public QrCodeGeneratorProxy(Binding binding, string remoteAddress) : base(binding, remoteAddress)
        {
        }
    }
}
