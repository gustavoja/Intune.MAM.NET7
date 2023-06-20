using System;

namespace Core.Authentication.Msal
{
    public class MsalConfiguration
    {
        /// <summary>
        /// Active directory ID (guid). It shall also be set on the Android Manifest
        /// </summary>
        internal const string AAD_CLIENT_ID = YOUR_AAD_CLIENT_ID;
        internal const string AAD_AUTHORITY_URI = "https://login.microsoftonline.com/" + AAD_CLIENT_ID;
        /// <summary>
        /// MSAL Android redirect URI "url msauth://intune.mam.net7/hash"
        /// </summary>
        internal const string MSAL_REDIRECT_URI = YOUR_MSAL_REDIRECT_URI;
        /// <summary>
        /// MSAL Azure application client ID (guid)
        /// </summary>
        internal const string MSAL_CLIENT_ID = YOUR_MSAL_CLIENT_ID;
        /// <summary>
        /// Intune scope id to request Intune API token from MSAL. This value can be also retrieved on <see cref="INTUNE.MAM.NET7.Intune.EnrollmentNotificationReceiver"/>
        /// </summary>
        internal const string INTUNE_MSAL_SCOPE = "https://msmamservice.api.application";
        //internal const string INTUNE_MSAL_SCOPE = "https://wip.mam.manage.microsoft.us";

        /// <summary>
        /// Intune scope permission
        /// </summary>
        internal const string INTUNE_MSAL_SCOPE_RESOURCE = "DeviceManagementManagedApps.ReadWrite";
        /// <summary>
        /// MSAL scope for login. IE: https://tenant.sharepoint.com/.default
        /// </summary>
        internal const string SHAREPOINT_MSAL_SCOPE = YOUR_SHAREPOINT_MSAL_SCOPE;

        public static bool MSAL_CONFIGURED => AAD_CLIENT_ID != DEFAULT_AAD_CLIENT_ID &&
                                           MSAL_REDIRECT_URI != DEFAULT_MSAL_REDIRECT_URI &&
                                           MSAL_CLIENT_ID != DEFAULT_MSAL_CLIENT_ID &&
                                           SHAREPOINT_MSAL_SCOPE != DEFAULT_SHAREPOINT_MSAL_SCOPE;
        /// <summary>
        /// Do not modify this value
        /// </summary>
        const string DEFAULT_AAD_CLIENT_ID = "default";
        /// <summary>
        /// Do not modify this value
        /// </summary>
        const string DEFAULT_MSAL_REDIRECT_URI = "default";
        /// <summary>
        /// Do not modify this value
        /// </summary>
        const string DEFAULT_MSAL_CLIENT_ID = "default";
        /// <summary>
        /// Do not modify this value
        /// </summary>
        const string DEFAULT_SHAREPOINT_MSAL_SCOPE = "default";

        /// Add your credentials here

        /// <summary>
        /// Active directory ID (guid). It shall also be set on the Android Manifest
        /// </summary>
        const string YOUR_AAD_CLIENT_ID = "default";
        /// <summary>
        /// MSAL Android redirect URI "url msauth://intune.mam.net7/hash"
        /// </summary>
        const string YOUR_MSAL_REDIRECT_URI = "default";
        /// <summary>
        /// MSAL Azure application client ID (guid)
        /// </summary>
        const string YOUR_MSAL_CLIENT_ID = "default";
        /// <summary>
        /// MSAL scope for login. IE: https://tenant.sharepoint.com/.default
        /// </summary>
        const string YOUR_SHAREPOINT_MSAL_SCOPE = "default";
    }
}

