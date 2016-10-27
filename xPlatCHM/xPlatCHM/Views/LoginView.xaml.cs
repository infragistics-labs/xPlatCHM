using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private void Button_Clicked(object sender, EventArgs e)
        {

            Navigation.PushAsync(new CasesView());
        }
    }
}
