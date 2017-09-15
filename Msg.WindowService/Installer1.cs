using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Msg.WindowService
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        private System.ServiceProcess.ServiceInstaller serviceInstaller1;
        private ServiceProcessInstaller processInstaller;
        public Installer1()
        {
            InitializeComponent();

            processInstaller = new ServiceProcessInstaller();
            serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            processInstaller.Account = ServiceAccount.LocalService;
            serviceInstaller1.StartType = ServiceStartMode.Automatic;//自动启动
            serviceInstaller1.ServiceName = "MsgAutoCancelOrders";
            serviceInstaller1.Description = "美速购自动取消过期订单";
            Installers.Add(serviceInstaller1);
            Installers.Add(processInstaller);

        }
    }
}
