using System;
using System.Collections.Generic;

namespace xPlatCHM.DataModels
{
	public class Case
	{
		public Customer Customer { get; set; }

		public string CaseNumber { get; set; }

		public string AssignedTo { get; set; }

		public string Component { get; set; }

		public DateTime CreatedOn { get; set; }

		public string Description { get; set; }

		public DateTime DueBy { get; set; }

		public string ForumThreadID { get; set; }

		public string ID { get; set; }

		public bool IsCaseVisibleToCustomer { get; set; }

		public Priority Priority { get; set; }

		public string Product { get; set; }

		public string ProductBuild { get; set; }

		public string SalesOwner { get; set; }

		public ServiceLevel ServiceLevel { get; set; }

		public Status Status { get; set; }

		public string StepsToReproduce { get; set; }

		public string Team { get; set; }

		public string Technology { get; set; }

		public string Title { get; set; }

		public IList<Activity> Activities { get; set; }
	}
}
