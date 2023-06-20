using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace Intune.MAM.NET7.Droid.UI
{
    internal class LogViewHolder : RecyclerView.ViewHolder
    {
        readonly TextView logTextView;

        public LogViewHolder(View itemView) : base(itemView)
        {
            logTextView = itemView.FindViewById<TextView>(Resource.Id.logItemTextView);
        }

        internal void SetText(string log)
        {
            logTextView.Text = log;
        }
    }
}
