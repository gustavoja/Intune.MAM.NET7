namespace Core.Authentication.Msal;

using Core.Logger;
using Microsoft.Identity.Client;
using System.Linq;
using System.Threading.Tasks;

internal class MsalClientService
{
    const bool ENABLE_MSAL_CLIENT_LOGS = false;
    static IPublicClientApplication pca;
    IPublicClientApplication PCA => pca ?? CreateMsalClient();

    ILogger logger;

    public MsalClientService(ILogger logger)
    {
        this.logger = logger;
    }

    IPublicClientApplication CreateMsalClient()
    {
        var builder = PublicClientApplicationBuilder.Create(MsalConfiguration.MSAL_CLIENT_ID)
            .WithAuthority(AzureCloudInstance.AzurePublic, MsalConfiguration.AAD_CLIENT_ID)
            .WithBroker()
            .WithLogging(new LogCallback((LogLevel level, string message, bool containsPii) =>
            {
                if(ENABLE_MSAL_CLIENT_LOGS)
                    logger.Log("Msal client", $"{level} - {message}");
            }), LogLevel.Info, true)
            .WithRedirectUri(MsalConfiguration.MSAL_REDIRECT_URI);

        pca = builder.Build();
        return pca;
    }

    public async Task<LoginResponse> LoginInteractive(object view)
    {
        var request = PCA.AcquireTokenInteractive(new[] { MsalConfiguration.SHAREPOINT_MSAL_SCOPE });

        request = request.WithParentActivityOrWindow(view)
                         .WithPrompt(Prompt.SelectAccount);

        AuthenticationResult result = null;

        try
        {
           result = await request.ExecuteAsync();
        }
        catch (Exception e)
        {
            logger.Log(nameof(MsalClientService), $"Failed to get MSAL token interactively {e.Message}");
        }

        if (result is null)
        {
            logger.Log(nameof(MsalClientService), $"Failed to get MSAL token interactively");
            return null;
        }


        logger.Log(nameof(MsalClientService), $"Successful MSAL interactive login");
        return new LoginResponse
        {
            UserName = result.Account.Username,
            AccountId = result.Account.HomeAccountId.ObjectId,
            TenantId = result.TenantId,
            Token = result.AccessToken
        };
    }
    public Task<LoginResponse> LoginSilent()
    {
        return AcquireToken(new List<string> { MsalConfiguration.SHAREPOINT_MSAL_SCOPE });
    }

    public Task<LoginResponse> AcquireIntuneToken(string resource= MsalConfiguration.INTUNE_MSAL_SCOPE)
    {
        logger.Log(nameof(MsalClientService), $"Acquiring Intune MAM token with resource {resource}");
        return AcquireToken(new List<string> { resource.TrimEnd('/') + "/" + MsalConfiguration.INTUNE_MSAL_SCOPE_RESOURCE.TrimStart('/') });
    }

    async Task<LoginResponse> AcquireToken(IEnumerable<string> scopes)
    {
        AuthenticationResult result = null;

        try
        {
            var accounts = await PCA.GetAccountsAsync();
            result = await PCA.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
        }
        catch (Exception e)
        {
            logger.Log(nameof(MsalClientService), $"Failed to get MSAL token silently {e.Message}");
            return null;
        }

        if (result is null)
        {
            logger.Log(nameof(MsalClientService), "Failed to get MSAL token silently");
            return null;
        }

        logger.Log(nameof(MsalClientService), "Successfully retrieved MSAL token silently");
        return new LoginResponse
        {
            UserName = result.Account.Username,
            AccountId = result.Account.HomeAccountId.ObjectId,
            TenantId = result.TenantId,
            Token = result.AccessToken
        };
    }


    public Task Logout()
    {
        return Task.Run(async () =>
        {
            var loggedAccounts = await PCA.GetAccountsAsync();
            foreach (var account in loggedAccounts)
                await PCA.RemoveAsync(account);
        });
    }
}