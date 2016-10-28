using System;

namespace xPlatCHM.ViewModels
{
	internal class SavedSearchViewModel
	{
		public string Title { get; set; }

		public string IconSource { get; set; }

		public int CasesDelta { get; set; }

		public string CasesDeltaImageSource { get; set; }

		public Type TargetType { get; set; }

		public bool IsSelected { get; set; }
	}
}
