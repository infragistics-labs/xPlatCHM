using System;

using Xamarin.Forms;

namespace xPlatCHM.Views
{
	public partial class LoginView : ContentPage
    {
        public LoginView()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void LoginBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CasesView());
        }
    }
}
