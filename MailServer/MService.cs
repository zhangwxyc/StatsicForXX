﻿using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topshelf;

namespace MailServer
{
    public class MService : ServiceControl, ServiceSuspend
    {
        private readonly Quartz.IScheduler scheduler;
        public MService()
        {
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
        }
        public bool Start(HostControl hostControl)
        {
            scheduler.Start();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            scheduler.Shutdown(false);
            return true;
        }

        public bool Continue(HostControl hostControl)
        {
            scheduler.ResumeAll();
            return true;
        }

        public bool Pause(HostControl hostControl)
        {
            scheduler.PauseAll();
            return true;
        }
    }
}
