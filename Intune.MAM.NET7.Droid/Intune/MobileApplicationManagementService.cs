using Core.Authentication;
using Core.Logger;
using Core.MobileApplicationManagement;
using Intune.MAM.NET7.Droid.Intune.NotificationsReceivers;
using Microsoft.Intune.Mam.Client.App;
using Microsoft.Intune.Mam.Client.Notification;
using Microsoft.Intune.Mam.Policy;
using Microsoft.Intune.Mam.Policy.Notification;
using System.Threading.Tasks;

namespace Intune.MAM.NET7.Droid.Intune
{
    internal class MobileApplicationManagementService : IMobileApplicationManagementService
    {
        readonly ILogger logger;
        readonly IAuthenticationService authenticationService;
        readonly Android.Content.Context context;

        TaskCompletionSource<bool> _enrollmentTask;
        TaskCompletionSource<bool> _unenrollmentTask;


        public MobileApplicationManagementService(ILogger logger, IAuthenticationService authenticationService, Android.Content.Context context)
        {
            this.logger = logger;
            this.authenticationService = authenticationService;
            this.context = context;
        }

        public bool IsEnrolled { get; private set; }

        public void Init()
        {
            // Register the MAMAuthenticationCallback as soon as possible.
            // This will handle acquiring the necessary access token for MAM.

            logger.Log(nameof(MobileApplicationManagementService), "Initializing");
            IMAMEnrollmentManager mgr = null;

            mgr = MAMComponents.Get<IMAMEnrollmentManager>();
            logger.Log(nameof(MobileApplicationManagementService), "IMAMEnrollmentManager retrieved");

            mgr.RegisterAuthenticationCallback(new MAMWEAuthCallback(authenticationService, logger));
            logger.Log(nameof(MobileApplicationManagementService), "MAMWEAuthCallback registered");

            // Register the notification receivers to receive MAM notifications.
            // Applications can receive notifications from the MAM SDK at any time.
            // More information can be found here: https://docs.microsoft.com/en-us/intune/app-sdk-android#register-for-notifications-from-the-sdk
            IMAMNotificationReceiverRegistry registry = MAMComponents.Get<IMAMNotificationReceiverRegistry>();

            logger.Log(nameof(MobileApplicationManagementService), "IMAMNotificationReceiverRegistry retrieved");

            foreach (MAMNotificationType notification in MAMNotificationType.Values())
            {
                registry.RegisterReceiver(new ToastNotificationReceiver(context), notification);
                logger.Log(nameof(MobileApplicationManagementService), $"{notification} registered on ToastNotificationReceiver");
            }

            registry.RegisterReceiver(new EnrollmentNotificationReceiver(context, logger, OnEnrollmentSuccess,OnEnrollmentFail, OnUnenrollmentSuccess, OnUnenrollmentFail), MAMNotificationType.MamEnrollmentResult);
            logger.Log(nameof(MobileApplicationManagementService), $"{MAMNotificationType.MamEnrollmentResult} registered on EnrollmentNotificationReceiver");

            registry.RegisterReceiver(new WipeNotificationReceiver(context), MAMNotificationType.WipeUserData);
            logger.Log(nameof(MobileApplicationManagementService), $"{MAMNotificationType.WipeUserData} registered on WipeNotificationReceiver");
        }


        public async Task<bool> Enroll(LoginResponse response)
        {
            logger.Log(nameof(MobileApplicationManagementService), "Starting Intune MAM enrollment");
            
            if (_enrollmentTask != null)
            {
                logger.Log(nameof(MobileApplicationManagementService), "Awaiting pending enrollment");
                await _enrollmentTask.Task;
            }

            IMAMEnrollmentManager mgr = MAMComponents.Get<IMAMEnrollmentManager>();
            var state = mgr.GetRegisteredAccountStatus(response.UserName, response.AccountId)?.Code;
            
            if(state.HasValue)
                IsEnrolled = state.Value == IMAMEnrollmentManager.Result.EnrollmentSucceeded.Code;

            logger.Log(nameof(MobileApplicationManagementService), $"Already enrolled? {IsEnrolled} {state}");

            if (IsEnrolled)
            {
                logger.Log(nameof(MobileApplicationManagementService), "Already enrolled");
                return IsEnrolled;
            }
            
            _enrollmentTask = new TaskCompletionSource<bool>();

            logger.Log(nameof(MobileApplicationManagementService), "IMAMEnrollmentManager retrieved");

            mgr.UpdateToken(response.UserName, response.TenantId, "",response.Token);
            mgr.RegisterAccountForMAM(response.UserName, response.AccountId, response.TenantId);
            logger.Log(nameof(MobileApplicationManagementService), "RegisterAccountForMAM requested");

            IsEnrolled = await _enrollmentTask.Task;
            _enrollmentTask = null;

            var msg = IsEnrolled ? "completed" : "failed";
            logger.Log(nameof(MobileApplicationManagementService), "Intune MAM Enrolment "+ msg);
            return IsEnrolled;
        }

        public async Task<bool> Unenroll(LoginResponse response)
        {
            logger.Log(nameof(MobileApplicationManagementService), "Starting Intune MAM unenrollment");

            if (_unenrollmentTask != null)
            {
                logger.Log(nameof(MobileApplicationManagementService), "Awaiting pending unenrollment");
                await _unenrollmentTask.Task;
            }

            IMAMEnrollmentManager mgr = MAMComponents.Get<IMAMEnrollmentManager>();
            var state = mgr.GetRegisteredAccountStatus(response.UserName, response.AccountId)?.Code;

            if (state.HasValue)
                IsEnrolled = state.Value == IMAMEnrollmentManager.Result.EnrollmentSucceeded.Code;

            if (!IsEnrolled)
            {
                logger.Log(nameof(MobileApplicationManagementService), "Already unenrolled");
                return true;
            }

            _unenrollmentTask = new TaskCompletionSource<bool>();

            logger.Log(nameof(MobileApplicationManagementService), "IMAMEnrollmentManager retrieved");

            mgr.UnregisterAccountForMAM(response.UserName, response.AccountId);
            logger.Log(nameof(MobileApplicationManagementService), "UnregisterAccountForMAM requested");

            IsEnrolled = !await _unenrollmentTask.Task;
            _unenrollmentTask = null;

            var msg = !IsEnrolled ? "completed" : "failed";
            logger.Log(nameof(MobileApplicationManagementService), "Intune MAM Unenrollment " + msg);
            return !IsEnrolled;
        }

        void OnEnrollmentSuccess() 
        {
            logger.Log(nameof(MobileApplicationManagementService), "Enrollment completed");
            _enrollmentTask?.TrySetResult(true);
        }

        void OnEnrollmentFail(IMAMEnrollmentNotification notification)
        {
            logger.Log(nameof(MobileApplicationManagementService), $"Enrollment failed  {notification.EnrollmentResult}");
            _enrollmentTask?.TrySetResult(false);
        }

        void OnUnenrollmentSuccess()
        {
            logger.Log(nameof(MobileApplicationManagementService), "Unenrollment completed");
            _unenrollmentTask?.TrySetResult(true);
        }

        void OnUnenrollmentFail(IMAMEnrollmentNotification notification)
        {
            logger.Log(nameof(MobileApplicationManagementService), $"Unenrollment failed {notification.EnrollmentResult}");
            _unenrollmentTask?.TrySetResult(false);
        }

    }
}
