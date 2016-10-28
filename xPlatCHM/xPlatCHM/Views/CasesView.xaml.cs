﻿using System.Collections.Generic;

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
					CasesDeltaImageSource = "up-arrow.png",
					TargetType = null
				};
				masterPageItems.Add(model);
			}

			return masterPageItems;
		}
	}
}
