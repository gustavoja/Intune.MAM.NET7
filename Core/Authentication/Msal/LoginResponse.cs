using System;

namespace Core.Authentication
{
    public class LoginResponse
    {
        public string UserName { get; set; }
        public string AccountId { get; set; }
        public string TenantId { get; set; }
        public string Token { get; internal set; }
    }
}

