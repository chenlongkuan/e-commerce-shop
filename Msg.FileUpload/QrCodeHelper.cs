using System.Configuration;
using Msg.Config;

namespace Msg.FileUpload
{
    public class QrCodeHelper
    {

        private static readonly string Key = ConfigurationManager.AppSettings["Key"].ToString();

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="text">输入文本内容</param>
        /// <returns>二维码图片地址</returns>
        public static string GenerateQrCode(string text)
        {
            string remoteAddress = WcfAddressConfigManager.GetConfig().NewFileAddress;


            var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;

            var proxy = new QrCodeGeneratorProxy(binding, remoteAddress);

            var gc = proxy.CreateChannel();


            var result = gc.Create(text);
            proxy.Dispose();

            return result;
        }

    }
}
