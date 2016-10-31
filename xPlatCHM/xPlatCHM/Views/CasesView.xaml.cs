using System;
using System.Collections.Generic;

using Xamarin.Forms;
using xPlatCHM.Services;
using xPlatCHM.ViewModels;

namespace xPlatCHM.Views
{
	public partial class CasesView : MasterDetailPage
    {
        public CasesView()
        {
            InitializeComponent();

			this.savedQueuesListView.ItemsSource = this.CreateSavedSearchesList();

			this.savedQueuesListView.ItemSelected += OnQueryChanged;
        }

		private void OnQueryChanged(object sender, SelectedItemChangedEventArgs e)
		{
			var item = e.SelectedItem as SavedSearchViewModel;
			if (item != null)
			{
				var casesPage = new CasePage();
				this.Detail = new NavigationPage(casesPage);
				this.IsPresented = false;

				casesPage.LoadCases(item.Title);
			}
		}

		private IList<SavedSearchViewModel> CreateSavedSearchesList()
		{
			var masterPageItems = new List<SavedSearchViewModel>();

			foreach (var savedSearch in DataService.Instance.GetSavedSearches())
			{
				var model = new SavedSearchViewModel()
				{
					Title = savedSearch,
					IconSource = "tag.png",
					IsSelected = false,
					CasesDelta = 3,
					CasesDeltaImageSource = "uparrow.png"
				};
				masterPageItems.Add(model);
			}

			return masterPageItems;
		}
	}
}
