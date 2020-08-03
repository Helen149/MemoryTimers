using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory.Timers
{
    public class Timer: IDisposable
    {
        public static string Report { get { return CreateReport(); }}
        private static int countTimers;
        private static Stopwatch timerRest = new Stopwatch();
        private static Dictionary<string, string> timersReport;
        private static string restReport;

        public static Timer Start(string name="*")
        {
            if (countTimers == 0)
            {
                restReport = null;
                timersReport = new Dictionary<string, string>();
            }  
            countTimers++;
            return new Timer(name);
        }

        Stopwatch timerDo;
        string name;
        int numberTimer;

        private Timer(string name)
        {
            numberTimer = Timer.countTimers;
            this.name = name;
            timersReport.Add(name, null);
            timerDo = new Stopwatch();
            timerRest.Reset();
            timerDo.Start();
        }

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    timerDo.Stop();
                    if (timerRest.IsRunning)
                    {
                        timerRest.Stop();
                        restReport += CreateLineReport("Rest", timerRest);
                        timerRest.Reset();
                    }
                    countTimers--;
                    if (countTimers != 0)
                        timerRest.Start();
                    timersReport[name] = CreateLineReport(name, timerDo);

                }
                disposedValue = true;
            }
        }

        ~Timer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        private string CreateLineReport(string name, Stopwatch timer)
        {
            var sizeWithMarginLeft = name.Length + 4 * countTimers;
            name = name.PadLeft(sizeWithMarginLeft);
            name = name.PadRight(20);
            return name + ": " + timer.ElapsedMilliseconds/10 *10 + "\n";
        }

        private static string CreateReport()
        {
            var result = new StringBuilder();
            foreach (var report in timersReport)
                result.Append(report.Value);
            result.Append(restReport);
            return result.ToString();
        }
    }
}
