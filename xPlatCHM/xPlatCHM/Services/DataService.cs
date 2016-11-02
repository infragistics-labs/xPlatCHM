using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using xPlatCHM.DataModels;

namespace xPlatCHM.Services
{
	internal class DataService
	{
		private const string CRMBaseAddress = "http://crm.en.staging.infragistics.local:8080/CHSHackWeekAPI/api/";
		private const string CasesRelativePath = "cases/";
		private const string SavedSearchesRelativePath = "savedsearches/";
		private const string CouldNotLoadCasesError = "Could not load cases.";
		private const string CouldNotLoadCaseError = "Could not load case details.";
		private const string CouldNotLoadSavedSearchesError = "Could not load saved searches.";
		private const string ActivitiesPropertyName = "Activities";
		private const string CustomerPropertyName = "Customer";
		private const string CompanyPropertyName = "Company";
		private static DataService instance;

		private IDictionary<string, Case[]> mapQueryToListOfCases = new Dictionary<string, Case[]>();

		private DataService() { }


		// TODO: use some dependency injection mechanism and remove this singleton
		public static DataService Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new DataService();
				}

				return instance;
			}
		}

		public IEnumerable<string> GetSavedSearches()
		{
			// TODO: enable this when the API works

			//using (var client = new HttpClient())
			//{
			//	var response = client.GetAsync(CRMBaseAddress + SavedSearchesRelativePath).Result;

			//	if (!response.IsSuccessStatusCode)
			//	{
			//		throw new InvalidOperationException(CouldNotLoadSavedSearchesError);
			//	}

			//	var responseText = response.Content.ReadAsStringAsync().Result;

			//	var searchesJArray = (JArray) JsonConvert.DeserializeObject(responseText);

			//	var searches = searchesJArray.Cast<string>();

			//	return searches;
			//}

			return new List<string>()
			{
				"In Queue",
				"In Progress",
				"Awaiting",
				"In Development",
				"My Active",
				"All in Queue"
			};
		}

		public IEnumerable<Case> GetCases(string queryName)
		{
			// TODO: Enable this code when the authentication is working
			//var cachedCases = this.GetCachedCases(queryName);
			//if (cachedCases.Any())
			//	return cachedCases;

			//return this.LoadCases(queryName);

			return this.CreateTestCases();
		}

		public IEnumerable<Case> LoadCases(string queryName)
		{
			using (var client = new HttpClient())
			{
				var auth = Views.LoginView.AuthResult;

				var response = client.GetAsync(CRMBaseAddress + CasesRelativePath).Result;

				if (!response.IsSuccessStatusCode)
				{
					throw new InvalidOperationException(CouldNotLoadCasesError);
				}

				var responseText = response.Content.ReadAsStringAsync().Result;

				var casesJArray = (JArray)JsonConvert.DeserializeObject(responseText);

				var cases = casesJArray.Select(x => this.CreateFromJObject((JObject)x)).ToArray();

				if (!string.IsNullOrEmpty(queryName))
				{
					this.mapQueryToListOfCases[queryName] = cases;
				}

				return cases;
			}
		}

		public IEnumerable<Case> GetCachedCases(string queryName)
		{
			if (string.IsNullOrEmpty(queryName))
				throw new ArgumentNullException(nameof(queryName));

			if (!this.mapQueryToListOfCases.ContainsKey(queryName))
				return Enumerable.Empty<Case>();

			return this.mapQueryToListOfCases[queryName];
		}

		public Case GetCaseDetails(string caseNumber)
		{
			using (var client = new HttpClient())
			{
				var response = client.GetAsync(CRMBaseAddress + CasesRelativePath + caseNumber).Result;

				if (!response.IsSuccessStatusCode)
				{
					throw new InvalidOperationException(CouldNotLoadCaseError);
				}

				var responseText = response.Content.ReadAsStringAsync().Result;

				var caseJObject = (JObject) JsonConvert.DeserializeObject(responseText);

				var currentCase = this.CreateFromJObject(caseJObject);
				return currentCase;
			}
		}

		private Case CreateFromJObject(JObject caseJObject)
		{
			var currentCase = new Case();

			currentCase.AssignedTo = caseJObject.Value<string>(nameof(currentCase.AssignedTo));
			currentCase.CaseNumber = caseJObject.Value<string>(nameof(currentCase.CaseNumber));
			currentCase.Component = caseJObject.Value<string>(nameof(currentCase.Component));
			currentCase.CreatedOn = caseJObject.Value<DateTime>(nameof(currentCase.CreatedOn));
			currentCase.Description = caseJObject.Value<string>(nameof(currentCase.Description));
			currentCase.DueBy = caseJObject.Value<DateTime>(nameof(currentCase.DueBy));
			currentCase.ForumThreadID = caseJObject.Value<string>(nameof(currentCase.ForumThreadID));
			currentCase.ID = caseJObject.Value<string>(nameof(currentCase.ID));
			currentCase.IsCaseVisibleToCustomer = caseJObject.Value<bool>(nameof(currentCase.IsCaseVisibleToCustomer));
			currentCase.Product = caseJObject.Value<string>(nameof(currentCase.Product));
			currentCase.ProductBuild = caseJObject.Value<string>(nameof(currentCase.ProductBuild));
			currentCase.SalesOwner = caseJObject.Value<string>(nameof(currentCase.SalesOwner));
			currentCase.StepsToReproduce = caseJObject.Value<string>(nameof(currentCase.StepsToReproduce));
			currentCase.Team = caseJObject.Value<string>(nameof(currentCase.Team));
			currentCase.Technology = caseJObject.Value<string>(nameof(currentCase.Technology));
			currentCase.Title = caseJObject.Value<string>(nameof(currentCase.Title));
			currentCase.Priority = (Priority)caseJObject.Value<int>(nameof(currentCase.Priority));
			currentCase.ServiceLevel = (ServiceLevel)caseJObject.Value<int>(nameof(currentCase.ServiceLevel));
			currentCase.Status = (Status)caseJObject.Value<int>(nameof(currentCase.Status));

			var customer = new Customer()
			{
				Name = caseJObject.Value<string>(CustomerPropertyName),
				Company = caseJObject.Value<string>(CompanyPropertyName)
			};
			currentCase.Customer = customer;

			currentCase.Activities = this.CreateActivities(caseJObject).ToList();

			return currentCase;
		}

		private IEnumerable<Activity> CreateActivities(JObject caseJObject)
		{
			var activitiesJArray = caseJObject.Value<JArray>(ActivitiesPropertyName);

			foreach (var activityJObj in activitiesJArray)
			{
				var activity = new Activity();
				activity.CreatedOn = activityJObj.Value<DateTime>(nameof(activity.CreatedOn));
				activity.Description = activityJObj.Value<string>(nameof(activity.Description));
				activity.From = activityJObj.Value<string>(nameof(activity.From));
				activity.ID = activityJObj.Value<string>(nameof(activity.ID));
				activity.IsVisibleToCustomer = activityJObj.Value<bool>(nameof(activity.IsVisibleToCustomer));
				activity.Status = activityJObj.Value<int>(nameof(activity.Status));
				activity.Subject = activityJObj.Value<string>(nameof(activity.Subject));
				activity.To = activityJObj.Value<string>(nameof(activity.To));

				yield return activity;
			}
		}

		private IEnumerable<Case> CreateTestCases()
		{
			yield return new Case
			{
				AssignedTo = "System User, Infragistics",
				CaseNumber = "CAS-177774-S8K2B5",
				Customer = new Customer
				{
					Name = "Xkbjye, Cgyqg",
					Company = "Project DocControl"
				},
				Component = "WebDataGrid",
				CreatedOn = DateTime.Parse("2016-10-05T13:56:23"),
				Description = "<p>I have a webdatagrid that has a list of companies. &nbsp;When I select a company, I need for the contacts list to update. &nbsp;The load event is not firing on my website. &nbsp;It's firing on my local web site, but when I upload the pages, it gives me a server does not respond error.</p>",
				DueBy = DateTime.Parse("2016-10-10T13:00:00"),
				ForumThreadID = null,
				ID = "8ea4ce04-258b-e611-924f-000c295bab63",
				IsCaseVisibleToCustomer = true,
				Priority = Priority.Normal,
				Product = "Infragistics ASP.NET 2016 Vol. 1",
				ProductBuild = null,
				SalesOwner = "Kowitz, Jason",
				ServiceLevel = ServiceLevel.Standard,
				Status = Status.InQueue,
				StepsToReproduce = "",
				Team = "DS - Web",
				Technology = "ASPNET",
				Title = "I'm trying to cascading dropdowns and it's not firing on the website"
			};

			yield return new Case
			{
				AssignedTo = "System User, Infragistics",
				CaseNumber = "CAS-177776-Y1W7B6",
				Customer = new Customer
				{
					Name = "oyasagmnr, uqykd",
					Company = "Casamba Inc."
				},
				Component = "Data Grid",
				CreatedOn = DateTime.Parse("2016-10-05T15:28:35"),
				Description = "<p>Hi,</p>< p ></ p >< p > Is there a way in Infragistics UWP Data grid to select cells instead of rows ?</ p >< p ></ p >< p > Thanks.</ p >",
				DueBy = DateTime.Parse("2016-10-06T15:00:00"),
				ForumThreadID = null,
				ID = "44d3c6e5-318b-e611-924f-000c295bab63",
				IsCaseVisibleToCustomer = false,
				Priority = Priority.Normal,
				Product = "Infragistics UWP August",
				ProductBuild = null,
				SalesOwner = "Haddad, Joseph",
				ServiceLevel = ServiceLevel.Trial,
				Status = Status.InQueue,
				StepsToReproduce = "",
				Team = "DS - Xsharp",
				Technology = "Other",
				Title = "Data Grid Selection mode cell"
			};

			yield return new Case
			{
				AssignedTo = "System User, Infragistics",
				CaseNumber = "CAS-177777-Z0D0B8",
				Customer = new Customer
				{
					Name = "Uoaruaw, Npri",
					Company = "NEW YORK LIFE"
				},
				Component = "WebDataGrid",
				CreatedOn = DateTime.Parse("2016-10-05T15:31:31"),
				Description = "<p>Team,</p><p>We have editable webdatagrid with batch update mode enabled.</p><p>Initially, user inserted 1st record and clicked on external save button, Record saved successfully to the database.&nbsp;</p><p>Now user is trying to insert 2nd record and clicked on external save button. As per business logic in that scenario, grid should contain only 1 record. so server side business logic thrown exception.&nbsp;</p><p>Then on client user removed the 2nd record and it is removing successfully on client side. But when user clicks on external save button, Still on server side grid is showing 2 records. We are not understanding why it is still showing 2 records.&nbsp;</p><p>Could you please let us know what to do be done for this. &nbsp;&nbsp;</p>",
				DueBy = DateTime.Parse("2016-10-06T15:00:00"),
				ForumThreadID = null,
				ID = "44cb384d-328b-e611-924f-000c295bab63",
				IsCaseVisibleToCustomer = true,
				Priority = Priority.Normal,
				Product = "Infragistics ASP.NET 2016 Vol. 1",
				ProductBuild = null,
				SalesOwner = "Marotta, Sam",
				ServiceLevel = ServiceLevel.Priority,
				Status = Status.InQueue,
				StepsToReproduce = "",
				Team = "DS - Web",
				Technology = "ASPNET",
				Title = "Webdatgrid data is not updating properly on postback"
			};
		}
	}
}
