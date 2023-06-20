using Core.Authentication;
using Core.Authentication.Msal;
using Core.Logger;
using Core.MobileApplicationManagement;
using Core.Reosurces;
using System.Collections.ObjectModel;

namespace Core.ViewModels
{
    public class MainViewModel
    {
        readonly IAuthenticationService authenticationService;
        readonly IMobileApplicationManagementService mobileApplicationManagementService;
        readonly ILogger logger;
        LoginResponse intuneLoginInfo;

        public MainViewModel(IAuthenticationService authenticationService,IMobileApplicationManagementService mobileApplicationManagementService, ILogger logger)
        {
            this.authenticationService = authenticationService;
            this.mobileApplicationManagementService = mobileApplicationManagementService;
            this.logger = logger;
            UpdateUIState();

            SilentLoginAndEnroll();
        }

        public string LoginButtonText { get; private set; }
        public string EnrollButtonText { get; private set; }
        public bool EnrollButtonEnabled { get; private set; }
        public string Username { get; private set; }
        public bool Loading { get; private set; }
        public string LoginAndEnrollmentStateText { get; private set; }

        public ObservableCollection<string> Logs => logger.Logs;

        public Action OnUIStateChanged { get;  set; }

        public async Task Login(object view)
        {
            Loading = true;
            UpdateUIState();

            if (!authenticationService.IsAuthenticated)
            {
                logger.Log(nameof(MainViewModel), "Trying to login");

                LoginAndEnrollmentStateText = Strings.Authenticating;
                UpdateUIState();

                await authenticationService.Login(view);

                var msg = authenticationService.IsAuthenticated ? "completed" : "failed";
                logger.Log(nameof(MainViewModel), "Login " + msg);
            }
            else
            {
                logger.Log(nameof(MainViewModel), "Trying to logout");

                LoginAndEnrollmentStateText = Strings.LoginOut;
                UpdateUIState();

                if (mobileApplicationManagementService.IsEnrolled)
                {
                    logger.Log(nameof(MainViewModel), "Enrolled, will first unenroll before login out");
                    await Enroll();
                }
                await authenticationService.Logout();

                var msg = !authenticationService.IsAuthenticated ? "completed" : "failed";
                logger.Log(nameof(MainViewModel), "Logout " + msg);
            }


            Loading = false;
            UpdateUIState();
        }

        public async Task Enroll()
        {
            Loading = true;
            UpdateUIState();

            if (authenticationService.IsAuthenticated)
            {
                if(!mobileApplicationManagementService.IsEnrolled)
                {
                    logger.Log(nameof(MainViewModel), $"Trying to enroll with Mobile Application Manager {authenticationService.IsAuthenticated}");

                    LoginAndEnrollmentStateText = Strings.Enrolling;
                    UpdateUIState();

                    intuneLoginInfo = await authenticationService.GetIntuneMamAuthToken(authenticationService.CurrentLoginInfo.UserName, authenticationService.CurrentLoginInfo.TenantId, MsalConfiguration.INTUNE_MSAL_SCOPE);


                    var isEnrolled = await mobileApplicationManagementService.Enroll(intuneLoginInfo);

                    var msg = isEnrolled ? "completed" : "failed";
                    logger.Log(nameof(MainViewModel), "Enrollment " + msg);
                }
                else if (intuneLoginInfo != null)
                {
                    LoginAndEnrollmentStateText = Strings.Unenrolling;
                    UpdateUIState();

                    var isEnrolled = await mobileApplicationManagementService.Unenroll(intuneLoginInfo);
                    intuneLoginInfo = null;

                    var msg = isEnrolled ? "completed" : "failed";
                    logger.Log(nameof(MainViewModel), "Unenrollment " + msg);
                }
            }

            Loading = false;
            UpdateUIState();
        }

        public void OnResume()
        {
            UpdateUIState();
        }

        void UpdateUIState()
        {
            LoginButtonText = authenticationService.IsAuthenticated ?  Strings.Logout : Strings.Login;
            EnrollButtonText = mobileApplicationManagementService.IsEnrolled ? Strings.Unenroll : Strings.Enroll;
            EnrollButtonEnabled = authenticationService.IsAuthenticated;
            Username = authenticationService.CurrentLoginInfo?.UserName;

            if (!Loading)
            {
                LoginAndEnrollmentStateText = authenticationService.IsAuthenticated ? Strings.Authenticated : Strings.NotAuthenticated;
                LoginAndEnrollmentStateText += $" {Strings.And} " + (mobileApplicationManagementService.IsEnrolled ? Strings.Enrolled : Strings.NotEnrolled);
            }

            OnUIStateChanged?.Invoke();
        }


        void SilentLoginAndEnroll()
        {
            logger.Log(nameof(MainViewModel), $"Trying to login silently and enroll with Mobile Application Manager");
            Loading = true;
            UpdateUIState();

            authenticationService.Login()
                .ContinueWith(async t => {
                    if (authenticationService.IsAuthenticated)
                    {
                        await Task.Delay(3000);
                        await Enroll();
                    }

                    Loading = false;
                    UpdateUIState();
                });
        }
    }
}
