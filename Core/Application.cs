using Core.Authentication;
using Core.Logger;
using Core.MobileApplicationManagement;

namespace Core
{
    public static class Application
    {
        static public IAuthenticationService AuthenticationService { get; private set; }
        static public ILogger Logger { get; private set; }
        static public IMobileApplicationManagementService MobileApplicationManagementService { get; private set; }

        public static void Init(ILogger logger)
        {
            Logger = logger;
            AuthenticationService = new AuthenticationService(logger);
        }

        public static void Register(IMobileApplicationManagementService mobileApplicationManagementService)
        {
            MobileApplicationManagementService = mobileApplicationManagementService;
        }
    }
}
