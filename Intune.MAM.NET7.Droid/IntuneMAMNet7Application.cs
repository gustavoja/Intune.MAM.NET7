using Android.App;
using Core.Authentication.Msal;
using Intune.MAM.NET7.Droid.Intune;
using Microsoft.Intune.Mam.Client.App;
using System;


namespace Intune.MAM.NET7.Droid
{
#if DEBUG
    /// <remarks>
    /// Due to an issue with debugging the Xamarin bound MAM SDK the Debuggable = false attribute must be added to the Application in order to enable debugging.
    /// Without this attribute the application will crash when launched in Debug mode. Additional investigation is being performed to identify the root cause.
    /// </remarks>
    [Application(Debuggable = false)]
#else
    [Application]
#endif
    class IntuneMAMNet7Application : MAMApplication
    {
        public IntuneMAMNet7Application(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base(handle, transfer) { }

        public override void OnMAMCreate()
        {
            if (!MsalConfiguration.MSAL_CONFIGURED)
                throw (new NotSupportedException("You shall add your MSAL configuration here ->" + nameof(MsalConfiguration)));
            var logger = new Logger();
            Core.Application.Init(logger);
            var mam = new MobileApplicationManagementService(logger, Core.Application.AuthenticationService, this);
            Core.Application.Register(mam);
            Core.Application.MobileApplicationManagementService.Init();


            base.OnMAMCreate();
        }

        public override void OnTerminate()
        {
            base.OnTerminate();

            // Delete all tasks and close the database connection.
        }
    }
}