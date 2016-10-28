using System.Linq;
using Xamarin.Forms;
using xPlatCHM.DataModels;
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

		public void LoadCases(string queryName)
		{
			var cases = DataService.Instance.GetCases(queryName);

			this.casesListView.ItemsSource = cases.Select(x => new CaseViewModel(x));
		}
    }
}
