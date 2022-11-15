using System;

namespace TestPages3.Controllers
{
    public interface IUpTimeService
    {
        string UpTimeSpan { get; }
    }
    public class UpTimeServiceSeconds: IUpTimeService
    {
        public static readonly DateTime timeStart;
        
        static UpTimeServiceSeconds()
        {
            timeStart = DateTime.Now;
        }

        public string UpTimeSpan
        {
            get { return $"uptime: {(int)(DateTime.Now - timeStart).TotalSeconds} s."; }
        }
    }
    public class UpTimeServiceMinutes: IUpTimeService
    {
        private DateTime timeStart;

        public UpTimeServiceMinutes()
        {
            timeStart = DateTime.Now;
        }

        public string UpTimeSpan
        {
            get { return $"uptime: {(int)(DateTime.Now - timeStart).TotalMinutes} min."; }
        }
    }
}