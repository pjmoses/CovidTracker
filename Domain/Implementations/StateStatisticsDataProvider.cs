using Domain.Interfaces;
using Domain.SystemObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace Domain.Implementations
{
	/// <summary>
	/// Implementation of <see cref="IStateStatisticsDataProvider"/> to get <see cref="StateStatistics"/>data
	/// from json objects at https://api.covidtracking.com 
	/// </summary>
	public class StateStatisticsDataProvider : IStateStatisticsDataProvider
	{
		#region Private Members

		const string DOMAIN = "https://api.covidtracking.com";

		const string STATE_TAG = "{state}";
		const string DATE_TAG = "{date}";

		readonly string ALL_STATES_CURRENT_DATE_ADDRESS = $"{DOMAIN}/v1/states/current.json";
		readonly string SINGLE_STATE_SINGLE_DATE_ADDRESS_FORMAT = $"{DOMAIN}/v1/states/{STATE_TAG}/{DATE_TAG}.json";

		#endregion [Private Members]

		#region Methods

		#region Public

		/// <summary>
		/// Gets a <see cref="StateStatistics"/> object for the latest date for all states
		/// </summary>
		/// <returns>A <see cref="StateStatistics"/> object for the latest date for all states</returns>
		public List<StateStatistics> GetCurrentStateStats()
		{
			string json = DownloadJsonText(ALL_STATES_CURRENT_DATE_ADDRESS);
			return DeserializeObject<List<StateStatistics>>(json);
		}

		/// <summary>
		/// Gets a <see cref="StateStatistics"/> object for the given <paramref name="date"/>
		/// for the state with the given <paramref name="stateCode"/>
		/// </summary>
		/// <param name="stateCode">The 2-letter code for the desired state</param>
		/// <param name="date">The date of the desired data</param>
		/// <returns>The <see cref="StateStatistics"/> object with the desired data or <see cref="null"/> if none is found</returns>
        public StateStatistics GetStateStatsByDate(string stateCode, DateTime date)
        {
			string formattedDate = date.ToString("yyyyMMdd");
			string address = GetWebAddressforSingleStateSingleDate(stateCode, formattedDate);

			string json = DownloadJsonText(address);
			return DeserializeObject<StateStatistics>(json);
		}

		#endregion [Public]

		#region Private

		/// <summary>
		/// Gets the web address of the json file containing data for the <paramref name="stateCode"/> and <paramref name="date"/>
		/// </summary>
		/// <param name="stateCode">State of the disired data</param>
		/// <param name="date">Date of the desired data</param>
		/// <returns>The web address for the desired json file</returns>
		private string GetWebAddressforSingleStateSingleDate(string stateCode, string date)
		{
			string adress = SINGLE_STATE_SINGLE_DATE_ADDRESS_FORMAT;
			adress = adress.Replace(STATE_TAG, stateCode.ToLower());
			adress = adress.Replace(DATE_TAG, date);
			return adress;
		}

		/// <summary>
		/// Gets the text of a json file located at the given <paramref name="address"/>
		/// </summary>
		/// <param name="address">The web address of the desired json file</param>
		/// <returns>The text of the desired json file at the given <paramref name="address"/></returns>
		/// <exception cref="ArgumentNullException">Thrown if the given <paramref name="address"/> is null</exception>
		private string DownloadJsonText(string address)
		{
			try
			{
				using var webClient = new WebClient();
				return webClient.DownloadString(address);
			}
			catch (WebException e) when (e.Message.Contains("(404)"))
			{
				// Page not found - assume bad date
				// No data for the given date, so return a json string representing no data
				return string.Empty;
			}
			catch (WebException e)
			{
				// Log message
				return null;
			}
			catch (NotSupportedException e)
			{
				// Log message
				return null;
			}
		}

		/// <summary>
		/// Deserializes the given <paramref name="json"/> to the given type <typeparamref name="T"/>
		/// </summary>
		/// <typeparam name="T">The type of the expected object in the <paramref name="json"/></typeparam>
		/// <param name="json">Text representing data to be deserialized</param>
		/// <returns></returns>
		private T DeserializeObject<T>(string json) where T : class
		{
			if (json is null)
			{
				return null;
			}

			try
			{
				return JsonConvert.DeserializeObject<T>(json);
			}
			catch (JsonReaderException e)
			{
				// poorly formatted input string
				// log exception
				return null;
			}
			catch (JsonSerializationException e)
			{
				// Incorrect output type
				// log exception
				return null;
			}
		}

		#endregion [Private]

		#endregion Methods
	}
}
