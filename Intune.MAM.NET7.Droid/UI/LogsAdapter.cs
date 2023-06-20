using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using System.Collections.ObjectModel;
using System.Linq;

namespace Intune.MAM.NET7.Droid.UI
{
    internal class LogsAdapter : RecyclerView.Adapter
    {
        readonly LayoutInflater? inflater;
        readonly ObservableCollection<string> logs;

        public LogsAdapter(Context context, ObservableCollection<string> logs)
        {
            inflater = LayoutInflater.FromContext(context);
            this.logs = logs;
        }
        public override int ItemCount =>logs.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var logViewHolder =holder as LogViewHolder;
            logViewHolder.SetText(logs.ElementAt(position));
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = inflater.Inflate(Resource.Layout.log_item, parent, false);
            return new LogViewHolder(view);
        }
    }
}
