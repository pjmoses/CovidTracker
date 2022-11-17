namespace CovidTrackerUi.Models
{
	/// <summary>
	/// Object to carry error data for the error page
	/// </summary>
	public class ErrorViewModel
	{
		/// <summary>
		/// An ID to display on the error page
		/// </summary>
		public string RequestId { get; set; }

		/// <summary>
		/// Whether to shoe the <see cref="RequestId"/>
		/// </summary>
		public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
	}
}
