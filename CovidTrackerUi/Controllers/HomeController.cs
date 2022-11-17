using CovidTrackerUi.Models;
using Domain.Interfaces;
using Domain.SystemObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CovidTrackerUi.Controllers
{
	/// <summary>
	/// View support for the attached models
	/// </summary>
	public class HomeController : Controller
	{
		#region Private Members

		private static readonly DateTime OLDEST_DATE = new DateTime(2020, 1, 13);
		private static readonly DateTime LATEST_DATE = new DateTime(2021, 3, 7);

		// Got down to the wire and didn't have time to learn to implement loggint.
		// The object was in the template, and I figured I'd get that implemented before completion. -Nope.
		private readonly ILogger<HomeController> _logger;
		private readonly IStateStatisticsDataProvider _dataProvider;
		private readonly MainPageViewModel _model;
		private static readonly List<string> _allStates;

		#endregion [Private Members]

		#region Constructors

		static HomeController()
		{
			_allStates = new List<string>()
			{
				"AK", "AL", "AR", "AS", "AZ", "CA", "CO", "CT", "DC", "DE", "FL", "GA", "GU", "HI", "IA", "ID",
				"IL", "IN", "KS", "KY", "LA", "MA", "MD", "ME", "MI", "MN", "MO", "MP", "MS", "MT", "NC", "ND",
				"NE", "NH", "NJ", "NM", "NV", "NY", "OH", "OK", "OR", "PA", "PR", "RI", "SC", "SD", "TN", "TX",
				"UT", "VA", "VI", "VT", "WA", "WI", "WV", "WY"
			};
		}

		public HomeController(ILogger<HomeController> logger, IStateStatisticsDataProvider dataProvider)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));

			_model = new MainPageViewModel();
		}

		#endregion [Constructors]

		#region Methods

		#region Public Actions

		public IActionResult Index()
		{
			List<StateStatistics> stateStats = _dataProvider.GetCurrentStateStats();
			stateStats = EnsureStatesOrderedByPositiveRates(stateStats);

			EnsureTheseStatesAreInTheMasterList(stateStats.Select(s => s.State));

			_model.StateStatistics = stateStats;
			_model.SelectedDate = GetCurrentDate(stateStats);
			_model.ShowState = new Dictionary<string, bool>();

			SetAllStatesInModelToShow();

			return View(_model);
		}

		[HttpPost]
		[ActionName("UpdateDate")]
		public IActionResult Index(DateTime newDate)
		{
			newDate = Clamp(OLDEST_DATE, newDate, LATEST_DATE);
			List<StateStatistics> stateStats = GetStatsForDay(newDate);
			stateStats = EnsureStatesOrderedByPositiveRates(stateStats);

			_model.StateStatistics = stateStats;
			_model.SelectedDate = newDate;
			_model.ShowState = new Dictionary<string, bool>();

			SetAllStatesInModelToShow();

			return View("Index", _model);
		}

		[HttpPost]
		[ActionName("FilterStates")]
		public IActionResult Index(MainPageViewModel oldModel,
			// This is a truely aweful solution.
			// The better thing to do is to ask someone
			// how to get the collections in the model to correctly propagate through posts.
			// Solutions to this problem I've found online failed, and I'm out of time.

			// Export the _allStates list to Excel to build this list and avoid copy-paste errors here and below. Solution still sucks.
			bool Include_AK, bool Include_AL, bool Include_AR, bool Include_AS, bool Include_AZ, bool Include_CA,
			bool Include_CO, bool Include_CT, bool Include_DC, bool Include_DE, bool Include_FL, bool Include_GA,
			bool Include_GU, bool Include_HI, bool Include_IA, bool Include_ID, bool Include_IL, bool Include_IN,
			bool Include_KS, bool Include_KY, bool Include_LA, bool Include_MA, bool Include_MD, bool Include_ME,
			bool Include_MI, bool Include_MN, bool Include_MO, bool Include_MP, bool Include_MS, bool Include_MT,
			bool Include_NC, bool Include_ND, bool Include_NE, bool Include_NH, bool Include_NJ, bool Include_NM,
			bool Include_NV, bool Include_NY, bool Include_OH, bool Include_OK, bool Include_OR, bool Include_PA,
			bool Include_PR, bool Include_RI, bool Include_SC, bool Include_SD, bool Include_TN, bool Include_TX,
			bool Include_UT, bool Include_VA, bool Include_VI, bool Include_VT, bool Include_WA, bool Include_WI,
			bool Include_WV, bool Include_WY)
		{
			// I shouldn't need to collect the data for the current date again if the model arrived here in tact.
			// But since it's not (and all the data is lost), I'm doing this.
			List<StateStatistics> stateStats = GetStatsForDay(oldModel.SelectedDate);
			stateStats = EnsureStatesOrderedByPositiveRates(stateStats);
			oldModel.StateStatistics = stateStats;
			oldModel.ShowState = new Dictionary<string, bool>();

			// My initial attempt at this aweful hack was to use reflection (relatively slow) to
			// iterate though the parameter list, checking the values with the matching names in _allStates,
			// but as it turns out, there is no way to get the method parameter values,
			// so I have to brute force it anyway. I hate this solution.

			// Holey Moses! I hate this!
			oldModel.ShowState["AK"] = Include_AK;
			oldModel.ShowState["AL"] = Include_AL;
			oldModel.ShowState["AR"] = Include_AR;
			oldModel.ShowState["AS"] = Include_AS;
			oldModel.ShowState["AZ"] = Include_AZ;
			oldModel.ShowState["CA"] = Include_CA;
			oldModel.ShowState["CO"] = Include_CO;
			oldModel.ShowState["CT"] = Include_CT;
			oldModel.ShowState["DC"] = Include_DC;
			oldModel.ShowState["DE"] = Include_DE;
			oldModel.ShowState["FL"] = Include_FL;
			oldModel.ShowState["GA"] = Include_GA;
			oldModel.ShowState["GU"] = Include_GU;
			oldModel.ShowState["HI"] = Include_HI;
			oldModel.ShowState["IA"] = Include_IA;
			oldModel.ShowState["ID"] = Include_ID;
			oldModel.ShowState["IL"] = Include_IL;
			oldModel.ShowState["IN"] = Include_IN;
			oldModel.ShowState["KS"] = Include_KS;
			oldModel.ShowState["KY"] = Include_KY;
			oldModel.ShowState["LA"] = Include_LA;
			oldModel.ShowState["MA"] = Include_MA;
			oldModel.ShowState["MD"] = Include_MD;
			oldModel.ShowState["ME"] = Include_ME;
			oldModel.ShowState["MI"] = Include_MI;
			oldModel.ShowState["MN"] = Include_MN;
			oldModel.ShowState["MO"] = Include_MO;
			oldModel.ShowState["MP"] = Include_MP;
			oldModel.ShowState["MS"] = Include_MS;
			oldModel.ShowState["MT"] = Include_MT;
			oldModel.ShowState["NC"] = Include_NC;
			oldModel.ShowState["ND"] = Include_ND;
			oldModel.ShowState["NE"] = Include_NE;
			oldModel.ShowState["NH"] = Include_NH;
			oldModel.ShowState["NJ"] = Include_NJ;
			oldModel.ShowState["NM"] = Include_NM;
			oldModel.ShowState["NV"] = Include_NV;
			oldModel.ShowState["NY"] = Include_NY;
			oldModel.ShowState["OH"] = Include_OH;
			oldModel.ShowState["OK"] = Include_OK;
			oldModel.ShowState["OR"] = Include_OR;
			oldModel.ShowState["PA"] = Include_PA;
			oldModel.ShowState["PR"] = Include_PR;
			oldModel.ShowState["RI"] = Include_RI;
			oldModel.ShowState["SC"] = Include_SC;
			oldModel.ShowState["SD"] = Include_SD;
			oldModel.ShowState["TN"] = Include_TN;
			oldModel.ShowState["TX"] = Include_TX;
			oldModel.ShowState["UT"] = Include_UT;
			oldModel.ShowState["VA"] = Include_VA;
			oldModel.ShowState["VI"] = Include_VI;
			oldModel.ShowState["VT"] = Include_VT;
			oldModel.ShowState["WA"] = Include_WA;
			oldModel.ShowState["WI"] = Include_WI;
			oldModel.ShowState["WV"] = Include_WV;
			oldModel.ShowState["WY"] = Include_WY;

			return View("Index", oldModel);
		}

		// This is an artifact of the template.
		// I'd use it, but I ran out of time.
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		#endregion [Public Actions]

		#region Private

		/// <summary>
		/// If the <paramref name="desiredTargetDate"/> lies before the <paramref name="latestDate"/>
		/// and before the <paramref name="oldestDate"/>, returns the <paramref name="desiredTargetDate"/>
		/// Otherwise, returns whichever date limitation is exceeded, checking the latest date first.
		/// </summary>
		/// <param name="oldestDate">The earliest date that can be returned</param>
		/// <param name="desiredTargetDate">The desired date</param>
		/// <param name="latestDate">The latest date that can be returned</param>
		/// <returns>
		/// The <paramref name="latestDate"/> if the <paramref name="desiredTargetDate"/> is after it.
		/// Otherwise the <paramref name="oldestDate"/> if the <paramref name="desiredTargetDate"/> is before it.
		/// Otherwise the <paramref name="desiredTargetDate"/>
		/// </returns>
		private DateTime Clamp(DateTime oldestDate, DateTime desiredTargetDate, DateTime latestDate)
		{
			if(desiredTargetDate > latestDate)
			{
				return latestDate;
			}

			if (desiredTargetDate < oldestDate)
			{
				return oldestDate;
			}

			return desiredTargetDate;
		}

		/// <summary>
		/// Returns the given <paramref name="stateStats"/> in descending order by the <see cref="StateStatistics.Positive"/> property.
		/// </summary>
		/// <param name="stateStats">The list to order</param>
		/// <returns>An ordered version of the <paramref name="stateStats"/></returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="stateStats"/> is null</exception>
		private List<StateStatistics> EnsureStatesOrderedByPositiveRates(List<StateStatistics> stateStats)
		{
			if (stateStats is null)
			{
				// TODO: Great place to log the problem
				throw new ArgumentNullException(nameof(stateStats));
			}

			return stateStats.OrderByDescending(stats => stats.Positive).ToList();
		}

		/// <summary>
		/// Ensures that the states found in <paramref name="otherStates"/> are all in the <see cref="_allStates"/> and adds them if they're not.
		/// </summary>
		/// <param name="otherStates">States that should be in <see cref="_allStates"/></param>
		private void EnsureTheseStatesAreInTheMasterList(IEnumerable<string> otherStates)
		{
			if (otherStates is null || !otherStates.Any())
			{
				return;
			}

			foreach (string state in otherStates.Where(state => !_allStates.Contains(state)))
			{
				_allStates.Add(state);
			}

			_allStates.Sort();
		}

		/// <summary>
		/// Gets the date of the first item in <paramref name="stateStats"/> with a valid date
		/// </summary>
		/// <param name="stateStats">A list of statistics to search for a date</param>
		/// <returns>The first valid date found</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="stateStats"/> is null</exception>
		private DateTime GetCurrentDate(List<StateStatistics> stateStats)
		{
			if (stateStats is null)
			{
				// TODO: Log this
				throw new ArgumentNullException(nameof(stateStats));
			}

			return stateStats.FirstOrDefault(state => state.Date > DateTime.MinValue)?.Date ?? DateTime.MinValue;
		}

		/// <summary>
		/// Gets the statistics for all states for the given <paramref name="newDate"/> from the <see cref="_dataProvider"/>
		/// </summary>
		/// <param name="newDate">The date of the new data to receive</param>
		/// <returns>A list of all state statistics for the given <paramref name="newDate"/></returns>
		private List<StateStatistics> GetStatsForDay(DateTime newDate)
		{
			SynchronizedCollection<StateStatistics> datedStateStats = new SynchronizedCollection<StateStatistics>();

			Parallel.ForEach(_allStates, state =>
			{
				StateStatistics stats = _dataProvider.GetStateStatsByDate(state, newDate);
				stats = ValidateStatistics(stats, newDate, state);
				datedStateStats.Add(stats);
			});

			return datedStateStats.ToList();
		}

		/// <summary>
		/// Sets the visibility state of all the states to show in the list
		/// </summary>
		private void SetAllStatesInModelToShow()
		{
			if (_model?.StateStatistics is null ||
				_model?.ShowState is null ||
				_allStates is null)
			{
				// TODO: Here's where I'd log a problem.
				return;
			}

			// Catch-all in case I missed one. I found that some pages have more states than others.
			EnsureTheseStatesAreInTheMasterList(_model.StateStatistics.Select(s => s.State));
			List<string> copyOfAllStates = new List<string>(_allStates);

			// Given that multiple threads can access the list, it is safer to iterate over a copy.
			foreach (string state in copyOfAllStates)
			{
				_model.ShowState[state] = true;
			}
		}

		/// <summary>
		/// If <paramref name="stats"/> is null,
		/// returns a <see cref="StateStatistics"/> object
		/// with only the <paramref name="validDate"/> and <paramref name="state"/>
		/// and all other properties null
		/// </summary>
		/// <param name="stats">The statistics object to validate</param>
		/// <param name="validDate">The date to use in the returned object should <paramref name="stats"/> be null</param>
		/// <param name="state">The state to use in the returned object should <paramref name="stats"/> be null</param>
		/// <returns>A non-null <see cref="StateStatistics"/> object</returns>
		private StateStatistics ValidateStatistics(StateStatistics stats, DateTime validDate, string state)
		{
			if (stats is null)
			{
				return new StateStatistics(validDate, state, null, null, null, null);
			}

			return stats;
		}

		#endregion [Private]

		#endregion Methods
	}
}
