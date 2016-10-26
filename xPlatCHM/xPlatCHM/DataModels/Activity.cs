using System;

namespace xPlatCHM.DataModels
{
	public class Activity
	{
		public DateTime CreatedOn { get; set; }

		public string Description { get; set; }

		public string From { get; set; }

		public string ID { get; set; }

		public bool IsVisibleToCustomer { get; set; }

		public int Status { get; set; }

		public string Subject { get; set; }

		public string To { get; set; }
	}
}
