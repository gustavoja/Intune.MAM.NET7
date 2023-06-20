using Core.Authentication;

namespace Core.MobileApplicationManagement
{
    public interface IMobileApplicationManagementService
    {
        public bool IsEnrolled { get; }
        void Init();
        public Task<bool> Enroll(LoginResponse response);
        public Task<bool> Unenroll(LoginResponse response);
    }
}
