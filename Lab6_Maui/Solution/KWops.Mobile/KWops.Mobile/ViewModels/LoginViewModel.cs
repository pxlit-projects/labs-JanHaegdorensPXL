using KWops.Mobile.Services;
using KWops.Mobile.Services.Identity;
using System;
using System.Windows.Input;

namespace KWops.Mobile.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private IIdentityService identityService;
        private ITokenProvider tokenProvider;
        private INavigationService navigationService;
        private IToastService toastService;

        public ICommand LoginCommand { get; }

        public LoginViewModel(IIdentityService identityService, ITokenProvider tokenProvider, INavigationService navigationService, IToastService toastService)
        {
            this.identityService = identityService;
            this.tokenProvider = tokenProvider;
            this.navigationService = navigationService;
            this.toastService = toastService;

            LoginCommand = new Command(async () => await ExecuteLoginCommand(), () => !IsBusy);
        }

        private async Task ExecuteLoginCommand()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var loginResult = await identityService.LoginAsync();

                if (loginResult.IsError)
                {
                    await toastService.DisplayToastAsync(loginResult.ErrorDescription);
                }
                else
                {
                    tokenProvider.AuthAccessToken = loginResult.AccessToken;
                    await navigationService.NavigateAsync("TeamsPage");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
