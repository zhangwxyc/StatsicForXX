using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Topshelf;

namespace MailServer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {



            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + @"\ConfigFile\log4net.config"));

            HostFactory.Run(x =>
            {
                x.Service<MService>();
                x.RunAsLocalSystem();
                x.SetDescription("Quartz+TopShelf实现Windows服务作业调度的一个示例Demo");
                x.SetDisplayName("QuartzTopShelfDemo服务");
                x.SetServiceName("QuartzTopShelfDemoService");
                x.EnablePauseAndContinue();
            });





            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[] 
            //{ 
            //    new Service1() 
            //};
            //ServiceBase.Run(ServicesToRun);
        }
    }
}
