using Domain.SystemObjects;
using System;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    /// <summary>
    /// Interface for a data provider to provide required <see cref="StateStatistics"/> objects
    /// </summary>
    public interface IStateStatisticsDataProvider
    {
        List<StateStatistics> GetCurrentStateStats();
        StateStatistics GetStateStatsByDate(string stateCode, DateTime date);
    }
}
