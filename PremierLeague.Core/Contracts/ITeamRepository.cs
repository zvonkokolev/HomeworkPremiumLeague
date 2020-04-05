using PremierLeague.Core.DataTransferObjects;
using PremierLeague.Core.Entities;
using System.Collections.Generic;
using System;
namespace PremierLeague.Core.Contracts
{
	public interface ITeamRepository
	{
		IEnumerable<Team> GetAllWithGames();
		IEnumerable<Team> GetAll();
		void AddRange(IEnumerable<Team> teams);
		Team Get(int teamId);
		void Add(Team team);
		Tuple<Team, int> TeamWithMostGoals();
		Tuple<Team, int> TeamWithMostHomeGoals();
		Tuple<Team, int> TeamWithMostAwayGoals();
		Tuple<Team, int> TeamWithBestRatio();
		TeamStatisticDto[] GetTeamStatistics();
		TeamTableRowDto[] GetTeamTable();
	}
}