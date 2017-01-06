using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MailServer
{
    public class Log
    {
        public static log4net.ILog Logger = log4net.LogManager.GetLogger("logger");
    }
}
