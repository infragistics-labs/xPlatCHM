using Acr.UserDialogs;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using xPlatCHM.Services;
using xPlatCHM.ViewModels;

namespace xPlatCHM.Views
{
	public partial class CasePage : ContentPage
    {
		public CasePage()
        {
            InitializeComponent();
        }

		public async Task LoadCases(string queryName)
		{
			UserDialogs.Instance.ShowLoading("Loading cases ...");

			var cases = await Task.Factory.StartNew(() =>
			{
				return DataService.Instance.GetCases(queryName);
			});

			this.casesListView.ItemsSource = cases.Select(x => new CaseViewModel(x));

			UserDialogs.Instance.HideLoading();
		}
    }
}
