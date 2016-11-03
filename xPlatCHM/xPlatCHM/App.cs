using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Xamarin.Forms;

namespace xPlatCHM
{
	public class App : Application
	{
		public IPlatformParameters PlatformParameters { get; set; }

		public App()
		{
            // The root page of your application
            var content = new Views.LoginView();
            MainPage = new NavigationPage(content);
        }

		protected override async void OnStart()
		{
			// Handle when your app starts

			// TODO: uncomment when the Azure login is working
			//await Views.LoginView.Authenticate(this.PlatformParameters);
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
