using System;
using System.Collections.Generic;
using System.Linq;
using PremierLeague.Core.Entities;
using Utils;
using System.IO;
using System.Text;

namespace PremierLeague.Core
{
	public static class ImportController
	{
		const string fileToReadFrom = "PremierLeague.csv";
		public static IEnumerable<Game> ReadFromCsv()
		{
			var lines = MyFile.ReadStringMatrixFromCsv(fileToReadFrom, false);
			List<Game> games = new List<Game>();
			Game game;
			Dictionary<string, Team> pairs = new Dictionary<string, Team>();
			foreach (var item in lines)
			{
				if (!pairs.TryGetValue(item[1], out Team teamH))
				{
					teamH = new Team { Name = item[1] };
					pairs.Add(item[1], teamH);
				}
				else
				{
					teamH.Name = item[1];
				}

				if (!pairs.TryGetValue(item[2], out Team teamA))
				{
					teamA = new Team { Name = item[2] };
					pairs.Add(item[2], teamA);
				}
				else
				{
					teamA.Name = item[2];
				}
				game = new Game()
				{
					Round = int.Parse(item[0]),
					HomeTeam = teamH,
					GuestTeam = teamA,
					HomeGoals = int.Parse(item[3]),
					GuestGoals = int.Parse(item[4])
				};
				games.Add(game);
			}
			return games;
		}
	}
}
