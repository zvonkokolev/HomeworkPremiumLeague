using System.Collections.Generic;
using System.Linq;
using PremierLeague.Core.Entities;
using Utils;
using System.IO;

namespace PremierLeague.Core
{
	public static class ImportController
	{
		const string fileToReadFrom = "PremierLeague.csv";
		public static IEnumerable<Game> ReadFromCsv()
		{
			var path = MyFile.GetFullNameInApplicationTree(fileToReadFrom);
			Dictionary<string, Team> teams = new Dictionary<string, Team>();
			teams = File.ReadAllLines(path)
				.Select(s => s.Split(';'))
				.Select(s => s[1])
				.Distinct()
				.ToDictionary(s => s, s => new Team { Name = s});

			var csv = File.ReadAllLines(path)
				.Select(s => s.Split(';'))
				.Select(s => new Game()
				{
					Round = int.Parse(s?[0]),
					HomeTeam = teams.TryGetValue(s?[1], out Team teamH) ? teamH : new Team { Name = s[1] },
					GuestTeam = teams.TryGetValue(s?[2], out Team teamA) ? teamA : new Team { Name = s[2] },
					HomeGoals = int.Parse(s?[3]),
					GuestGoals = int.Parse(s?[4])
				});
			return csv;
		}
	}
}
