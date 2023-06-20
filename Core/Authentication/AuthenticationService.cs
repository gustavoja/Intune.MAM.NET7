using Core.Authentication.Msal;
using Core.Logger;
using Core.ViewModels;

namespace Core.Authentication
{
    internal class AuthenticationService : IAuthenticationService
    {
        readonly ILogger logger;

        public AuthenticationService(ILogger logger)
        {
            this.logger = logger;
        }

        public LoginResponse CurrentLoginInfo { get; private set; }

        public bool IsAuthenticated { get; private set; }

        public async Task<LoginResponse> GetIntuneMamAuthToken(string upn, string aadId, string intuneResourceUrl)
        {
            logger.Log(nameof(AuthenticationService), $"Acquiring token for Intune user: {upn}\n aaid: {aadId}\nresourceId: {intuneResourceUrl}");

            MsalClientService msalClient = new(logger);
            var response = await msalClient.AcquireIntuneToken(intuneResourceUrl);

            if(response != null)
                logger.Log(nameof(AuthenticationService), $"Acquired token for Intune user: {response.UserName}\naaid: {response.AccountId}\nresourceId: {response.TenantId}\ntoken: {response.Token}");

            return response;
        }

        public async Task<LoginResponse> Login(object view=null)
        {
            MsalClientService msalClient = new(logger);

            logger.Log(nameof(AuthenticationService), "Login silently");
            var result = await msalClient.LoginSilent();
            if (result != null)
                logger.Log(nameof(AuthenticationService), "Silently logged in");
            else if (view != null)
            {
                logger.Log(nameof(MainViewModel), "Login interactively");
                result = await msalClient.LoginInteractive(view);
                if (result != null)
                    logger.Log(nameof(AuthenticationService), $"Interactively logged in");
            }

            if (result != null)
            {
                IsAuthenticated = true;
                CurrentLoginInfo = result;
                logger.Log(nameof(AuthenticationService), $"Login credentials \nUpn: {result.UserName}\nAAID: {result.AccountId}\nResourceID: {result.TenantId}\nToken: {result.Token}");

            }
            else
            {
                IsAuthenticated = false;
                logger.Log(nameof(AuthenticationService), $"Login failed");
            }

            return result;
        }

        public async Task Logout()
        {
            MsalClientService msalClient = new(logger);
            await msalClient.Logout();
            IsAuthenticated = false;
            CurrentLoginInfo = null;
        }
    }
}
