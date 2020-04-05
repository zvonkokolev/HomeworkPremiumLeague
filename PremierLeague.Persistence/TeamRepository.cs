using Microsoft.EntityFrameworkCore;
using PremierLeague.Core.Contracts;
using PremierLeague.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System;
using PremierLeague.Core.DataTransferObjects;

namespace PremierLeague.Persistence
{
	public class TeamRepository : ITeamRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public TeamRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public IEnumerable<Team> GetAllWithGames() => 
			_dbContext.Teams.Include(t => t.HomeGames).Include(t => t.AwayGames).ToList();

		public IEnumerable<Team> GetAll() => _dbContext.Teams.OrderBy(t => t.Name).ToList();

		public void AddRange(IEnumerable<Team> teams)
		{
			_dbContext.Teams.AddRange(teams);
		}

		public Team Get(int teamId) => _dbContext.Teams.Find(teamId);

		public void Add(Team team)
		{
			_dbContext.Teams.Add(team);
		}
		public Tuple<Team, int> TeamWithMostGoals()
		{
			var temp = _dbContext.Teams.Select(s => new
			{
				Team = s,
				TotalGoals = s.HomeGames.Sum(g => g.HomeGoals)
			  + s.AwayGames.Sum(g => g.GuestGoals)
			})
				.OrderByDescending(s => s.TotalGoals).First();
			return Tuple.Create(temp.Team, temp.TotalGoals);
		}
		public Tuple<Team, int> TeamWithMostHomeGoals()
		{
			var temp = _dbContext.Teams.Select(s => new
			{
				Team = s,
				HG = s.HomeGames.Sum(g => g.HomeGoals)
			})
			.OrderByDescending(s => s.HG).First();
			Tuple<Team, int> t = new Tuple<Team, int>(temp.Team, temp.HG);
			return t;
		}
		public Tuple<Team, int> TeamWithMostAwayGoals()
		{
			var temp = _dbContext.Teams.Select(s => new
			{
				Team = s,
				AG = s.AwayGames.Sum(g => g.GuestGoals)
			})
			.OrderByDescending(s => s.AG).First();
			Tuple<Team, int> t = new Tuple<Team, int>(temp.Team, temp.AG);
			return t;
		}
		public Tuple<Team, int> TeamWithBestRatio()
		{
			var temp = _dbContext.Teams.Select(s => new
			{
				Team = s,
				Ratio = s.HomeGames.Sum(t => t.HomeGoals)
				- s.HomeGames.Sum(t => t.GuestGoals)
				+ s.AwayGames.Sum(t => t.GuestGoals)
				- s.AwayGames.Sum(t => t.HomeGoals)
			}).OrderByDescending(t => t.Ratio).First();
			Tuple<Team, int> t = new Tuple<Team, int>(temp.Team, temp.Ratio);
			return t;
		}
		public TeamStatisticDto[] GetTeamStatistics() => _dbContext.Teams.Select(s => new TeamStatisticDto
		{
			Name = s.Name,
			AvgGoalsShotAtHome = s.HomeGames.Average(t => t.HomeGoals),
			AvgGoalsShotOutwards = s.AwayGames.Average(t => t.GuestGoals),
			AvgGoalsShotInTotal = s.HomeGames.Average(t => t.HomeGoals) + s.AwayGames.Average(t => t.GuestGoals),
			AvgGoalsGotAtHome = s.HomeGames.Average(t => t.GuestGoals),
			AvgGoalsGotOutwards = s.AwayGames.Average(t => t.HomeGoals),
			AvgGoalsGotInTotal = s.HomeGames.Average(t => t.GuestGoals) + s.AwayGames.Average(t => t.HomeGoals)
		})
				.OrderByDescending(t => t.AvgGoalsShotInTotal).ToArray();

		public TeamTableRowDto[] GetTeamTable()
		{
			int i = 1;
			var x = _dbContext.Teams.Select(s => new TeamTableRowDto
			{
				Id = s.Id,
				Name = s.Name,
				Matches = s.HomeGames.Count + s.AwayGames.Count,
				Won = s.HomeGames.Where(t => t.HomeGoals > t.GuestGoals).Count()
				+ s.AwayGames.Where(t => t.HomeGoals < t.GuestGoals).Count(),
				Lost = s.HomeGames.Where(t => t.HomeGoals < t.GuestGoals).Count()
				+ s.AwayGames.Where(t => t.HomeGoals > t.GuestGoals).Count(),
				GoalsFor = s.HomeGames.Sum(t => t.HomeGoals) + s.AwayGames.Sum(t => t.GuestGoals),
				GoalsAgainst = s.HomeGames.Sum(t => t.GuestGoals) + s.AwayGames.Sum(t => t.HomeGoals)
			}).AsEnumerable().OrderByDescending(t => t.Points).ThenByDescending(t => t.GoalDifference).ToArray();
			foreach (var item in x)
			{
				item.Rank = i++;
			}
			return x;
		}
	}
}