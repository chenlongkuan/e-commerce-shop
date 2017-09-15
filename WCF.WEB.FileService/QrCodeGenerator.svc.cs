using System;
using System.Drawing;
using System.Text;
using ThoughtWorks.QRCode.Codec;

namespace WCF.WEB.FileService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“QrCodeGenerator”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 QrCodeGenerator.svc 或 QrCodeGenerator.svc.cs，然后开始调试。
    public class QrCodeGenerator : IQrCodeGenerator
    {
        public string Create(string text)
        {
            Bitmap bt;
            var enCodeString = text;
            var qrCodeEncoder = new QRCodeEncoder();
            bt = qrCodeEncoder.Encode(enCodeString, Encoding.UTF8);
            var filename = "qrcode" + DateTime.Now.ToString("yyyymmddhhmmss") + ".jpg";
            var filePath = AppDomain.CurrentDomain.BaseDirectory + "//QrCode";
            bt.Save(filePath + filename);
            var fileUrl = "http://file.meisugou.com/" + filePath + filename;
            return fileUrl;
        }
    }
}
