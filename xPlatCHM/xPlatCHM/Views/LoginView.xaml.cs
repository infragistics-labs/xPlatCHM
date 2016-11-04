using Acr.UserDialogs;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using xPlatCHM.Common;
using xPlatCHM.Services;

namespace xPlatCHM.Views
{
	public partial class LoginView : ContentPage
    {
		// TODO: implement some dependency injection mechanism and remove this static properties

		public static AuthenticationResult AuthResult { get; private set; }

		public static User LoggedUser { get; private set; }

        public LoginView()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

		public static async Task Authenticate(IPlatformParameters parameters)
		{
			string clientId = "0583a69c-0da8-446d-9bd4-a4b06a72cd40";
			Uri returnUri = new Uri("https://crm.infragistics.com/InfragisticsInc/main.aspx");

			var infragisticsAuthority = "https://login.windows.net/5467154b-1b75-4824-9369-916e4fb500db/oauth2/token";

			var _authenticationContext = new AuthenticationContext(infragisticsAuthority, false);
			AuthResult = await _authenticationContext.AcquireTokenAsync(clientId, clientId, returnUri, parameters);
		}

		private async void LoginBtn_Clicked(object sender, EventArgs e)
		{
			UserDialogs.Instance.ShowLoading("Logging in ...");

			var loginResult = await Task.Factory.StartNew(() =>
			{
				if (this.CheckUserCredentials(this.usernameEntry.Text, this.passwordEntry.Text))
				{
					LoggedUser = new User(this.usernameEntry.Text, this.passwordEntry.Text);
					return true;
				}
				else
				{
					return false;
				}
			});

			this.lblIncorrectCredentials.IsVisible = !loginResult;

			UserDialogs.Instance.HideLoading();

			if (loginResult)
			{
				Navigation.PushAsync(new CasesView());
			}
		}

		private bool CheckUserCredentials(string email, string password)
		{
			try
			{
				var savedSearches = DataService.Instance.GetSavedSearches(email, password);
				return savedSearches.Any();
			}
			catch
			{
				return false;
			}
		}
	}
}
