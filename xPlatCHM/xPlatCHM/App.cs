using Xamarin.Forms;
using xPlatCHM.Services;

namespace xPlatCHM
{
	public class App : Application
	{
		public App()
		{
			// The root page of your application
			var content = new ContentPage
			{
				Title = "xPlatCHM",
				Content = new StackLayout
				{
					VerticalOptions = LayoutOptions.Center,
					Children = {
						new Label {
							HorizontalTextAlignment = TextAlignment.Center,
							Text = "Welcome to Xamarin Forms!"
						},
						new Button
						{
							Text = "Load cases",
							Command = new Command(this.LoadCases)
						},
						new Button
						{
							Text = "Load case details",
							Command = new Command(this.LoadCaseDetails)
						}
					}
				}
			};

			MainPage = new NavigationPage(content);
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}

		private void LoadCases()
		{
			var cases = DataService.Instance.GetCases("InQueue");
		}

		private void LoadCaseDetails()
		{
			var caseDetails = DataService.Instance.GetCaseDetails("CAS-177774-S8K2B5");
		}
	}
}
