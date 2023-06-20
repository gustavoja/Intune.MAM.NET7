using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using Microsoft.Intune.Mam.Client.Notification;
using Microsoft.Intune.Mam.Policy.Notification;

namespace Intune.MAM.NET7.Droid.Intune.NotificationsReceivers
{
    /// <summary>
    /// Receives wipe notifications from the Intune service and deletes task data.
    /// See: https://docs.microsoft.com/en-us/intune/app-sdk-android#mamnotificationreceiver
    /// </summary>
    class WipeNotificationReceiver : Java.Lang.Object, IMAMNotificationReceiver
    {
        Context context;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="context"></param>
        public WipeNotificationReceiver(Context context)
        {
            this.context = context;
        }

        /// <summary>
        /// Handles the incoming wipe notification.
        /// </summary>
        /// <param name="notification">Incoming notification.</param>
        /// <returns>
        /// The receiver should return true if it handled the notification without error(or if it decided to ignore the notification). 
        /// If the receiver tried to take some action in response to the notification but failed to complete that action it should return false.
        /// </returns>
        public bool OnReceive(IMAMNotification notification)
        {
            Log.Info(GetType().Name, "Performing application wipe and clearing the app database.");

            Handler handler = new Handler(context.MainLooper);
            handler.Post(() => { Toast.MakeText(context, "Performing application wipe", ToastLength.Short).Show();});

            return true;
        }
    }
}