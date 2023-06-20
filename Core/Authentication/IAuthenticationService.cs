namespace Core.Authentication
{
    public interface IAuthenticationService
    {
        bool IsAuthenticated { get; }
        LoginResponse CurrentLoginInfo { get; }
        Task<LoginResponse> Login(object loginView = null);
        Task<LoginResponse> GetIntuneMamAuthToken(string upn, string aadId, string intuneResourceUrl);
        Task Logout();
    }
}
