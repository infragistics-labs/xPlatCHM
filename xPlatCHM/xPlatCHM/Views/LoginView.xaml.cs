using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace xPlatCHM.Views
{
	public partial class LoginView : ContentPage
    {
		public static AuthenticationResult AuthResult { get; private set; }

        public LoginView()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

		public static async Task Authenticate(IPlatformParameters parameters)
		{
			// TODO: authentication is currently not working probably because the app in Azure is not properly configured
			//string resource = "https://crm.infragistics.com/InfragisticsInc/main.aspx";

			//string clientId = "0583a69c-0da8-446d-9bd4-a4b06a72cd40";
			//string commonAuthority = "https://login.windows.net/common";
			////Uri returnUri = new Uri("http://crm.en.staging.infragistics.local:8080/signin-microsoft");
			//Uri returnUri = new Uri("https://crm.infragistics.com/InfragisticsInc/signin-microsoft");

			//var authContext = new AuthenticationContext(commonAuthority, false);
			//if (authContext.TokenCache.ReadItems().Count() > 0)
			//	authContext = new AuthenticationContext(authContext.TokenCache.ReadItems().First().Authority);

			//AuthResult = await authContext.AcquireTokenAsync(resource, clientId, returnUri, parameters);
		}

		private async void LoginBtn_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new CasesView());
		}
	}
}
