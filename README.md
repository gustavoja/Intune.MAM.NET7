# Intune.MAM.NET7
Mobile application to test Intune Mobile Application Management on .NET7

Uses Intune MAM for MAUI library
Microsoft.Intune.Maui.Essentials.android
https://www.nuget.org/packages/Microsoft.Intune.Maui.Essentials.android/9.5.2-beta2

Relies on MSAL authentication for login and obtain Intune required token

Currently can compile on PC only. On MAC is no possible to build the project.
https://github.com/msintuneappsdk/intune-app-sdk-xamarin/issues/61

This application is conceived to test the login and enrollment
Policies
At this moment only out of the box policies (passive policies) can be tested such as but not limited to:
- Copy and paste block
- Screenshot block
- Access control policy (security code for the application) 
- Minimun app version check
- Minimum OS version/patch level check
- Disabling third party keyboard
- Runtime integrity control
- Jailbreak/root detection
  
More development is needed to test active policies such as but not limited to:
- Prevention to "Open in" or share
- Block print
- Data encryption

In order to succesfully run this project you need an Azure suscription with:
- An Azure Active Directory configured with users
- Azure MSAL authentication configured for this application
- An Intune suscription and a MAM license assigned to a user.
- Configure the MsalConfiguration static class with the required credentials (If this class is not configured it will throw and exception on launch)
