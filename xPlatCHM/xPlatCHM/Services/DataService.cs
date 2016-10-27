using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using xPlatCHM.DataModels;

namespace xPlatCHM.Services
{
	internal class DataService
	{
		private const string CRMBaseAddress = "http://crm.en.staging.infragistics.local:8080/CHSHackWeekAPI/api/";
		private const string CasesRelativePath = "cases/";
		private const string SavedSearchesRelativePath = "savedsearches/";
		private const string CouldNotLoadCasesError = "Could not load cases.";   // TODO: provide a better message
		private const string CouldNotLoadSavedSearchesError = "Could not load saved searches.";   // TODO: provide a better message

		private static DataService instance;

		private IDictionary<string, Case[]> mapQueryToListOfCases = new Dictionary<string, Case[]>();

		private DataService() { }

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
			using (var client = new HttpClient())
			{
				var response = client.GetAsync(CRMBaseAddress + SavedSearchesRelativePath).Result;

				if (!response.IsSuccessStatusCode)
				{
					throw new InvalidOperationException(CouldNotLoadSavedSearchesError);
				}

				var responseText = response.Content.ReadAsStringAsync().Result;

				var searchesJArray = (JArray) JsonConvert.DeserializeObject(responseText);

				var searches = searchesJArray.Cast<string>();

				return searches;
			}
		}

		public IEnumerable<Case> GetCases(string queryName)
		{
			try
			{
				return this.GetCachedCases(queryName);
			}
			catch (ArgumentOutOfRangeException ex) when (ex.ParamName == nameof(queryName))
			{
				return this.LoadCases(queryName);
			}
		}

		public IEnumerable<Case> LoadCases(string queryName)
		{
			using (var client = new HttpClient())
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
				throw new ArgumentOutOfRangeException(nameof(queryName));

			return this.mapQueryToListOfCases[queryName];
		}

		private Case CreateFromJObject(JObject jObject)
		{
			var currentCase = new Case();

			currentCase.AssignedTo = jObject.Value<string>(nameof(currentCase.AssignedTo));
			currentCase.CaseNumber = jObject.Value<string>(nameof(currentCase.CaseNumber));
			currentCase.Component = jObject.Value<string>(nameof(currentCase.Component));
			currentCase.CreatedOn = jObject.Value<DateTime>(nameof(currentCase.CreatedOn));
			currentCase.Description = jObject.Value<string>(nameof(currentCase.Description));
			currentCase.DueBy = jObject.Value<DateTime>(nameof(currentCase.DueBy));
			currentCase.ForumThreadID = jObject.Value<string>(nameof(currentCase.ForumThreadID));
			currentCase.ID = jObject.Value<string>(nameof(currentCase.ID));
			currentCase.IsCaseVisibleToCustomer = jObject.Value<bool>(nameof(currentCase.IsCaseVisibleToCustomer));
			currentCase.Product = jObject.Value<string>(nameof(currentCase.Product));
			currentCase.ProductBuild = jObject.Value<string>(nameof(currentCase.ProductBuild));
			currentCase.SalesOwner = jObject.Value<string>(nameof(currentCase.SalesOwner));
			currentCase.StepsToReproduce = jObject.Value<string>(nameof(currentCase.StepsToReproduce));
			currentCase.Team = jObject.Value<string>(nameof(currentCase.Team));
			currentCase.Technology = jObject.Value<string>(nameof(currentCase.Technology));
			currentCase.Title = jObject.Value<string>(nameof(currentCase.Title));
			currentCase.Priority = (Priority)jObject.Value<int>(nameof(currentCase.Priority));
			currentCase.ServiceLevel = (ServiceLevel)jObject.Value<int>(nameof(currentCase.ServiceLevel));
			currentCase.Status = (Status)jObject.Value<int>(nameof(currentCase.Status));

			var customer = new Customer()
			{
				Name = jObject.Value<string>("Customer"),
				Company = jObject.Value<string>("Company")
			};
			currentCase.Customer = customer;

			// TODO: activities

			return currentCase;
		}
	}
}
