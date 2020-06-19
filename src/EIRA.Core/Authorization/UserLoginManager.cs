using Abp.Domain.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace EIRA.Authorization
{
    public class UserLoginManager : IDomainService
    {
        private readonly IAuthenticationManager _authenticationManager;
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        static UserLoginManager()
        {
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
        }

        public UserLoginManager(IAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        public void MobileUserSingOut()
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }

    }
}
