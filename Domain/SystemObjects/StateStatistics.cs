using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.SystemObjects
{
	/// <summary>
	/// A group of data describing COVID data for a given state on a given date
	/// </summary>
	public class StateStatistics
	{
		#region Constructors

		/// <summary>
		/// Constructor intended to be called by a json deserializer
		/// </summary>
		/// <param name="date">
		/// An integer representing the date that the statistics are for
		/// in YYYYMMDD format and therefore should be 8 digits long
		/// </param>
		/// <param name="state"><inheritdoc cref="State" path="/summary"/></param>
		/// <param name="total"><inheritdoc cref="Total" path="/summary"/></param>
		/// <param name="positive"><inheritdoc cref="Positive" path="/summary"/></param>
		/// <param name="negative"><inheritdoc cref="Negative" path="/summary"/></param>
		/// <param name="hospitalized"><inheritdoc cref="Hospitalized" path="/summary"/></param>
		[JsonConstructor]
		public StateStatistics(
			int date,
			string state,
			int? total,
			int? positive,
			int? negative,
			int? hospitalized) : this(ConvertIntToDateTime(date), state, total, positive, negative, hospitalized) { }

		/// <summary>
		/// Instantiates a new instance of the <see cref="StateStatistics"/> class with full parameters
		/// </summary>
		/// <param name="date"><inheritdoc cref="Date" path="/summary"/></param>
		/// <param name="state"><inheritdoc cref="State" path="/summary"/></param>
		/// <param name="total"><inheritdoc cref="Total" path="/summary"/></param>
		/// <param name="positive"><inheritdoc cref="Positive" path="/summary"/></param>
		/// <param name="negative"><inheritdoc cref="Negative" path="/summary"/></param>
		/// <param name="hospitalized"><inheritdoc cref="Hospitalized" path="/summary"/></param>
		public StateStatistics(
			DateTime date,
			string state,
			int? total,
			int? positive,
			int? negative,
			int? hospitalized)
		{
			Date = date;
			State = state;
			Total = total;
			Positive = positive;
			Negative = negative;
			Hospitalized = hospitalized;
		}

		#endregion [Constructors]

		#region Properties

		/// <summary>
		/// The date that the statistics are for
		/// </summary>
		[DisplayFormat(DataFormatString = "{0:d}")]
		public DateTime Date { get; }

		/// <summary>
		/// A 2-digit code representing a state that the statistics belongs to
		/// </summary>
		public string State { get; }

		/// <summary>
		/// Total number of people reported by the state tested for COVID-19 on the given <see cref="Date"/> or null if no data is available
		/// </summary>
		public int? Total { get; }

		/// <summary>
		/// The number of confirmed plus probable cases of COVID-19 reported by the state> or null if no data is available
		/// </summary>
		public int? Positive { get; }

		/// <summary>
		/// The number of unique people who tested negative for COVID-19 or null if no data is available
		/// </summary>
		public int? Negative { get; }

		/// <summary>
		/// The number of people hospitalized with COVID-19 or null if no data is available
		/// </summary>
		public int? Hospitalized { get; }

		#endregion [Properties]

		#region Methods

		/// <summary>
		/// Converts an 8-digit int assumed to be in YYYYMMDD format to a <see cref="DateTime"/> object
		/// </summary>
		/// <remarks>
		/// If the <paramref name="date"/> is not an 8-digit int, then <see cref="DateTime.MinValue"/> will be returned.
		/// </remarks>
		/// <param name="date">The number in YYYYMMDD format to be converted to a <see cref="DateTime"/> object</param>
		/// <returns>
		/// A <see cref="DateTime"/> object represented by the given <paramref name="date"/>
		/// or <see cref="DateTime.MinValue"/> if the <paramref name="date"/> is not an 8-digit number
		/// </returns>
		private static DateTime ConvertIntToDateTime(int date)
		{
			string dateString = date.ToString();

			if (dateString.Length != 8)
			{
				return DateTime.MinValue;
			}

			// convert YYYYMMDD format to YYYY/MM/DD format to make it parsable by the DateTime.TryParse method
			dateString = $"{dateString[0..4]}/{dateString[4..6]}/{dateString[6..8]}";

			return DateTime.TryParse(dateString, out DateTime dt) ?
				dt :
				DateTime.MinValue;
		}

		#endregion [Methods]
	}
}
