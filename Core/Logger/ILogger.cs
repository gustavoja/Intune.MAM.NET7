using System.Collections.ObjectModel;

namespace Core.Logger
{
    public interface ILogger
    {
        public void Log(string tag, string message);

        public ObservableCollection<string> Logs { get; }
    }
}
