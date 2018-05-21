using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EC.Windows.Task.Scheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine($"*.exe http://google.com - POST to http://google.com");
                Console.WriteLine($"*.exe PARAM add - Add current exe to Task to Scheduler with PARAM");
            }
            try
            {
                if ((args != null) && (args.Length == 2))
                {
                    var action = new Microsoft.Win32.TaskScheduler.ExecAction(Assembly.GetExecutingAssembly().Location);
                    action.Arguments = args[0];
                    var trigger = Microsoft.Win32.TaskScheduler.Trigger.CreateTrigger(Microsoft.Win32.TaskScheduler.TaskTriggerType.Daily);
                    trigger.StartBoundary = DateTime.Now.Date;
                    trigger.Repetition.Duration = TimeSpan.FromMinutes(0);
                    trigger.Repetition.Interval = TimeSpan.FromMinutes(1);
                    Microsoft.Win32.TaskScheduler.TaskService.Instance.AddTask("Process", trigger, action);
                    return;
                }
                if ((args != null) && (args.Length == 1))
                {
                    using (var wc = new WebClient())
                    {
                        var s = wc.DownloadString(args[0]);
                    }
                }
            }
            catch(Exception exc)
            {

            }
        }
    }
}
