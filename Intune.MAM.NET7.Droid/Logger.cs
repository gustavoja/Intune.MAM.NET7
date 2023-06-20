using Core.Logger;

namespace Intune.MAM.NET7.Droid
{
    internal class Logger : BaseLogger
    {
        public override void Log(string tag, string message)
        {
            base.Log(tag, message);
            Android.Util.Log.Info(tag, message);
        }
    }
}
