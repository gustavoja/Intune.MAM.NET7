using System;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using Core.Authentication;
using Core.Logger;
using Microsoft.Intune.Mam.Client.Notification;
using Microsoft.Intune.Mam.Policy;
using Microsoft.Intune.Mam.Policy.Notification;

namespace Intune.MAM.NET7.Droid.Intune.NotificationsReceivers
{
    /// <summary>
    /// Receives enrollment notifications from the Intune service and performs the corresponding action for the enrollment result.
    /// See: https://docs.microsoft.com/en-us/intune/app-sdk-android#mamnotificationreceiver
    /// </summary>
    class EnrollmentNotificationReceiver : Java.Lang.Object, IMAMNotificationReceiver
    {
        readonly ILogger logger;
        readonly Context context;
        readonly Action onEnrollmentSuccess;
        readonly Action<IMAMEnrollmentNotification> onEnrolmentFail;
        readonly Action onUnenrollmentSuccess;
        readonly Action<IMAMEnrollmentNotification> onUnenrollmentFail;

        public EnrollmentNotificationReceiver(Context context, ILogger logger, Action onEnrollmentSuccess, Action<IMAMEnrollmentNotification> onEnrolmentFail, Action onUnenrollmentSuccess, Action<IMAMEnrollmentNotification> onUnenrollmentFail)
        {
            this.context = context;
            this.logger = logger;
            this.onEnrollmentSuccess = onEnrollmentSuccess;
            this.onEnrolmentFail = onEnrolmentFail;
            this.onUnenrollmentSuccess = onUnenrollmentSuccess;
            this.onUnenrollmentFail = onUnenrollmentFail;
        }

        /// <summary>
        /// When using the MAM-WE APIs found in IMAMEnrollManager, your app wil receive 
        /// IMAMEnrollmentNotifications back to signal the result of your calls.
        /// 
        /// More information can be found here: https://docs.microsoft.com/en-us/intune/app-sdk-android#result-and-status-codes
        /// </summary>
        /// <param name="notification">The notification that was received.</param>
        /// <returns>
        /// The receiver should return true if it handled the notification without error(or if it decided to ignore the notification). 
        /// If the receiver tried to take some action in response to the notification but failed to complete that action it should return false.
        /// 
        /// Notification types
        /// AuthorizationNeeded: 0
        /// NotLicensed: 1
        /// EnrollmentSucceeded: 2
        /// EnrollmentFailed: 3
        /// WrongUser: 4
        /// MdmEnrolled: 5
        /// UnenrollmentSucceeded: 6
        /// UnenrollmentFailed: 7
        /// Pending: 8
        /// CompanyPortalRequired: 9
        /// </returns>
        public bool OnReceive(IMAMNotification notification)
        {
            if (notification.Type != MAMNotificationType.MamEnrollmentResult)
                return true;

            IMAMEnrollmentNotification enrollmentNotification = notification.JavaCast<IMAMEnrollmentNotification>();

            var name = enrollmentNotification.EnrollmentResult.ToString();
            var resultCode = enrollmentNotification.EnrollmentResult.Code;
            string upn = enrollmentNotification.UserIdentity;

            string message = string.Format($"Received MAM Enrollment result {resultCode}:{name} for user {upn}.");
            logger.Log(GetType().Name, message);

            Handler handler = new Handler(context.MainLooper);
            handler.Post(() => { Toast.MakeText(context, message, ToastLength.Long).Show(); });

            if (resultCode.Equals(IMAMEnrollmentManager.Result.AuthorizationNeeded.Code))
            {
                logger.Log(GetType().Name, $"Authorization needed");
                onEnrolmentFail.Invoke(enrollmentNotification);
                return true;
            }
            else if (resultCode.Equals(IMAMEnrollmentManager.Result.NotLicensed.Code))
            {
                logger.Log(GetType().Name, "Not licensed. Check if this user has an Intune license assigned");
                onEnrolmentFail.Invoke(enrollmentNotification);
                return false;
            }
            else if (resultCode.Equals(IMAMEnrollmentManager.Result.EnrollmentSucceeded.Code))
            {
                logger.Log(GetType().Name, "Successfully enrolled with Intune MAM");
                onEnrollmentSuccess.Invoke();
                return true;
            }
            else if (resultCode.Equals(IMAMEnrollmentManager.Result.EnrollmentFailed.Code))
            {
                logger.Log(GetType().Name, "Failed to enroll with Intune MAM");
                onEnrolmentFail.Invoke(enrollmentNotification);
                return false;
            }
            else if (resultCode.Equals(IMAMEnrollmentManager.Result.WrongUser.Code))
            {
                logger.Log(GetType().Name, "Wrong user for Intune MAM");
                onEnrolmentFail.Invoke(enrollmentNotification);
                return false;
            }
            else if (resultCode.Equals(IMAMEnrollmentManager.Result.MdmEnrolled.Code))
            {
                logger.Log(GetType().Name, "MDM is enrolled");
                onEnrolmentFail.Invoke(enrollmentNotification);
                return true;
            }
            else if (resultCode.Equals(IMAMEnrollmentManager.Result.UnenrollmentSucceeded.Code))
            {
                logger.Log(GetType().Name, "Successfully unenrolled with Intune MAM");
                onUnenrollmentSuccess.Invoke();
                BlockUser(handler, $"User access to  corporate data should be blocked");
                return true;
            }
            else if (resultCode.Equals(IMAMEnrollmentManager.Result.UnenrollmentFailed.Code))
            {
                logger.Log(GetType().Name, "Failed to unenroll with Intune MAM");
                onUnenrollmentFail.Invoke(enrollmentNotification);
                return false;
            }
            else if (resultCode.Equals(IMAMEnrollmentManager.Result.Pending.Code))
            {
                logger.Log(GetType().Name, "Enrollment pending");
                return true;
            }
            else if (resultCode.Equals(IMAMEnrollmentManager.Result.CompanyPortalRequired.Code))
            {
                logger.Log(GetType().Name, "Company portal requited for enrollment");
                return true;
            }
            else
            {
                logger.Log(GetType().Name, $"Unknown code {name}:{resultCode} for user {upn}");
                return false;
            }
        }

        /// <summary>
        /// Blocks the user from accessing the application.
        /// </summary>
        /// <remarks>
        /// In a real application, the user would need to be blocked from proceeding forward and accessing corporate data. 
        /// In this sample app, we ask them politely to stop.
        /// </remarks>
        /// <param name="handler">Associated handler.</param>
        /// <param name="blockMessage">Message to display to the user.</param>
        void BlockUser(Handler handler, string blockMessage)
        {
            Log.Error(GetType().Name, blockMessage);
            handler.Post(() => { Toast.MakeText(context, blockMessage, ToastLength.Long).Show(); });
        }
    }
}