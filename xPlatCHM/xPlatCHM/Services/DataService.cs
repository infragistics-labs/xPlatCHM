using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
		private const string SavedSearchPropertyName = "Name";
		private static DataService instance;

		private IDictionary<string, Case[]> mapQueryToListOfCases = new Dictionary<string, Case[]>();

		private NetworkCredential networkCredential;

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

		private NetworkCredential NetworkCredential
		{
			get
			{
				if (this.networkCredential == null)
				{
					this.networkCredential = new NetworkCredential(Views.LoginView.LoggedUser.Email, Views.LoginView.LoggedUser.Password);
				}

				return this.networkCredential;
			}
		}

		public IEnumerable<string> GetSavedSearches(string email, string password)
		{
			var handler = new HttpClientHandler { Credentials = new NetworkCredential(email, password) };
			using (var client = new HttpClient(handler))
			{
				var response = client.GetAsync(CRMBaseAddress + SavedSearchesRelativePath).Result;

				if (!response.IsSuccessStatusCode)
				{
					throw new InvalidOperationException(CouldNotLoadSavedSearchesError);
				}

				var responseText = response.Content.ReadAsStringAsync().Result;

				var searchesJArray = (JArray) JsonConvert.DeserializeObject(responseText);

				var searches = searchesJArray.Select(x => x.Value<string>(SavedSearchPropertyName));

				return searches;
			}
		}

		public IEnumerable<string> GetSavedSearches()
		{
			return this.GetSavedSearches(Views.LoginView.LoggedUser.Email, Views.LoginView.LoggedUser.Password);
		}

		public IEnumerable<Case> GetCases(string queryName)
		{
			var cachedCases = this.GetCachedCases(queryName);
			if (cachedCases.Any())
				return cachedCases;

			return this.LoadCases(queryName);
		}

		public IEnumerable<Case> LoadCases(string queryName)
		{
			var handler = new HttpClientHandler { Credentials = this.NetworkCredential };

			using (var client = new HttpClient(handler))
			{
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
			var handler = new HttpClientHandler { Credentials = this.NetworkCredential };
			using (var client = new HttpClient(handler))
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
	}
}
