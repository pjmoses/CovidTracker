using Domain.SystemObjects;
using System;
using System.Collections.Generic;

namespace CovidTrackerUi.Models
{
	/// <summary>
	/// Object to carry data for the main page
	/// </summary>
	public class MainPageViewModel
	{
		/// <summary>
		/// A collection of statistics about each state
		/// </summary>
		public List<StateStatistics> StateStatistics { get; set; }

		/// <summary>
		/// The date that the user selected for the state statistics
		/// </summary>
		public DateTime SelectedDate { get; set; }

		/// <summary>
		/// A dictionary defining whether to show the states which are the keys
		/// </summary>
		public Dictionary<string, bool> ShowState { get; set; }
	}
}
