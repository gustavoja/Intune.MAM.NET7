using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Core.Logger
{
    public class BaseLogger : ILogger
    {
        public ObservableCollection<string> Logs { get; set; } = new();

        public virtual void Log(string tag, string message)
        {
            Trace.WriteLine($"{message}",$"[{tag}]");

            var msg = $"[{tag}]\n{message}";
            Logs.Insert(0,msg);
        }
    }
}
