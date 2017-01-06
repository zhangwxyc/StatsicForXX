using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MailServer
{
    public class MailJob : IJob,IDisposable
    {
        public MailLib.MailReceiveHelper MailReader { get; set; }
        public MailJob()
        {
            //MailReader = new MailLib.MailReceiveHelper();
            //MailReader.Init();
            Log.Logger.Debug("init");
        }
        public void Execute(IJobExecutionContext context)
        {
            //var info = MailReader.Receive("cc");
            Log.Logger.Debug("test");
        }

        public void Dispose()
        {
           // MailReader.Disconnect();
        }
    }
}
