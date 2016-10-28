using xPlatCHM.DataModels;

namespace xPlatCHM.ViewModels
{
	internal class CaseViewModel
	{
		private Case _case;

		public string CustomerName
		{
			get
			{
				return this._case.Customer.Name;
			}
		}

		public string CaseNumber
		{
			get
			{
				return this._case.CaseNumber;
			}
		}

		public string Component
		{
			get
			{
				return this._case.Component;
			}
		}

		public bool IsCaseVisibleToCustomer
		{
			get
			{
				return this._case.IsCaseVisibleToCustomer;
			}
		}

		public string Priority
		{
			get
			{
				return this._case.Priority.ToString();
			}
		}

		public string Product
		{
			get
			{
				return this._case.Product;
			}
		}

		public string ServiceLevel
		{
			get
			{
				return this._case.ServiceLevel.ToString();
			}
		}

		public string Status
		{
			get
			{
				return this._case.Status.ToString();
			}
		}

		public string Technology
		{
			get
			{
				return this._case.Technology;
			}
		}

		public string Title
		{
			get
			{
				return this._case.Title;
			}
		}

		public CaseViewModel(Case currentCase)
		{
			this._case = currentCase;
		}
	}
}
