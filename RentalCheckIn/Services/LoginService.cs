using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace RentalCheckIn.Services
{
    public class LoginService
    {
        private readonly ProtectedLocalStorage localStorage;
        private readonly ILocalStorageService localStorage1;

        public LoginService(ProtectedLocalStorage localStorage, ILocalStorageService localStorage1) 
        {
            this.localStorage = localStorage;
            this.localStorage1 = localStorage1;
        }
        public string GetToken()
        {
            //var accessToken = localStorage.GetAsync<string>("token").ToString();
            //return accessToken;
            var accessToken1 = localStorage1.GetItemAsStringAsync("authToken").ToString();
            return accessToken1;
        }

    }
}
