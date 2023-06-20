using Core.Authentication;
using Core.Logger;
using Microsoft.Intune.Mam.Policy;

namespace Intune.MAM.NET7.Droid.Intune
{
    /// <summary>
    /// Required by the MAM SDK. A token may be needed very early in the app lifecycle so the ideal
    /// place to register the callback is in the OnMAMCreate() method of the app's implementation
    /// of IMAMApplication.
    /// See https://docs.microsoft.com/en-us/intune/app-sdk-android#account-authentication
    /// </summary>º
    class MAMWEAuthCallback : Java.Lang.Object, IMAMServiceAuthenticationCallback
    {
        readonly IAuthenticationService authenticationService;
        ILogger logger;

        public MAMWEAuthCallback(IAuthenticationService authenticationService, ILogger logger)
        {
            this.authenticationService = authenticationService;
            this.logger = logger;
        }

        public string AcquireToken(string upn, string aadId, string resourceId)
        {
            logger.Log(GetType().Name, $"Providing token via the callback for aadID: {aadId} and resource ID: {resourceId}");

            var token = authenticationService.GetIntuneMamAuthToken(upn,aadId,resourceId).Result?.Token;

            logger.Log(GetType().Name, $"Providing token via the callback for Intune : {token}");
            return token;
        }
    }
}