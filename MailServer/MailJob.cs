using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MailServer
{
    public class MailJob : IJob
    {
        public MailLib.MailReceiveHelper MailReader { get; set; }
        public MailJob()
        {
            MailReader = new MailLib.MailReceiveHelper();
            MailReader.Init();
        }
        public void Execute(IJobExecutionContext context)
        {
            var info = MailReader.Receive("cc");
        }
    }
}
