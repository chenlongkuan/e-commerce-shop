using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Msg.WindowService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string[] args)
        {

            if (!Environment.UserInteractive)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
            { 
                new Service1() 
            };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                var ss = new Service1();
                ss.StartService(args);
                Console.WriteLine("Press any key to stop the program");
                Console.Read();

            }
        }
    }
}
