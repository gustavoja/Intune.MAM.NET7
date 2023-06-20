using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using Microsoft.Intune.Mam.Client.Notification;
using Microsoft.Intune.Mam.Policy.Notification;

namespace Intune.MAM.NET7.Droid.Intune.NotificationsReceivers
{
    /// <summary>
    /// Receives notifications from the Intune service and displays toast.
    /// See: https://docs.microsoft.com/en-us/intune/app-sdk-android#mamnotificationreceiver
    /// </summary>
    class ToastNotificationReceiver : Java.Lang.Object, IMAMNotificationReceiver
    {
        readonly Context context;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="context"> the context of the receiver.</param>
        public ToastNotificationReceiver(Context context)
        {
            this.context = context;
        }

        /// <summary>
        /// Handles the incoming notification.
        /// </summary>
        /// <param name="notification">Incoming notification.</param>
        /// <returns>True.</returns>
        public bool OnReceive(IMAMNotification notification)
        {
            var message = "Received MAMNotification of type " + notification.Type;
            Log.Info(GetType().Name, message);
            Handler handler = new Handler(context.MainLooper);
            handler.Post(() => { Toast.MakeText(context, message, ToastLength.Short).Show(); });
            return true;
        }
    }
}