using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MailServer
{
    public static class LoggerFactory
    {
        private static ILog _log = LogManager.GetLogger("logger");
        public static ILog GetLog()
        {
            return _log;
        }
    }
}
