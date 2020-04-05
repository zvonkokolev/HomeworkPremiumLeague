using PremierLeague.Core;
using PremierLeague.Core.Contracts;
using PremierLeague.Persistence;
using Serilog;
using System;
using System.Linq;
using ConsoleTables;
using System.Collections.Generic;

namespace PremierLeague.ImportConsole
{
	class Program
	{
		static void Main()
		{
			PrintHeader();
			InitData();
			AnalyzeData();

			Console.Write("Beenden mit Eingabetaste ...");
			Console.ReadLine();
		}

		private static void PrintHeader()
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(new String('-', 60));

			Console.WriteLine(
					@"
            _,...,_
          .'@/~~~\@'.          
         //~~\___/~~\\        P R E M I E R  L E A G U E 
        |@\__/@@@\__/@|             
        |@/  \@@@/  \@|            (inkl. Statistik)
         \\__/~~~\__//
          '.@\___/@.'
            `""""""
                ");

			Console.WriteLine(new String('-', 60));
			Console.WriteLine();
			Console.ResetColor();
		}

		/// <summary>
		/// Importiert die Ergebnisse (csv-Datei >> Datenbank).
		/// </summary>
		private static void InitData()
		{
			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				Log.Information("Import der Spiele und Teams in die Datenbank");

				Log.Information("Datenbank löschen");
				unitOfWork.DeleteDatabase();

				Log.Information("Datenbank migrieren");
				unitOfWork.MigrateDatabase();

				Log.Information("Spiele werden von premierleague.csv eingelesen");
				var games = ImportController.ReadFromCsv().ToArray();
				if (games.Length == 0)
				{
					Log.Warning("!!! Es wurden keine Spiele eingelesen");
				}
				else
				{
					Log.Debug($"  Es wurden {games.Count()} Spiele eingelesen!");
					var teams = games.Select(s => s.HomeTeam).Distinct().Union(games.Select(s => s.GuestTeam).Distinct());
					Log.Debug($"  Es wurden {teams.Count()} Teams eingelesen!");
					Log.Information("Daten werden in Datenbank gespeichert (in Context übertragen)");
					unitOfWork.Games.AddRange(games);
					unitOfWork.Teams.AddRange(teams);
					unitOfWork.SaveChanges();
					Log.Information("Daten wurden in DB gespeichert!");
				}
			}
		}
		private static void AnalyzeData()
		{
			using (IUnitOfWork unitOfWork = new UnitOfWork())
			{
				string caption = "Team mit den meisten geschossenen Toren:";
				string result = "";
				var res = unitOfWork.Teams.TeamWithMostGoals();
				result = $"{res.Item1.Name}: {res.Item2} Tore";
				PrintResult(caption, result, false);
				caption = "Team mit den meisten geschossenen Auswärtstoren:";
				res = unitOfWork.Teams.TeamWithMostAwayGoals();
				result = $"{res.Item1.Name}: {res.Item2} Auswärtstore";
				PrintResult(caption, result, false);
				caption = "Team mit den meisten geschossenen Heimtoren:";
				res = unitOfWork.Teams.TeamWithMostHomeGoals();
				result = $"{res.Item1.Name}: {res.Item2} Heimtore";
				PrintResult(caption, result, false);
				res = unitOfWork.Teams.TeamWithBestRatio();
				result = $"{res.Item1.Name}: {res.Item2} Tore";
				caption = "Team mit dem besten Torverhältnis:";
				PrintResult(caption, result, false);

				var dto = unitOfWork.Teams.GetTeamStatistics();
				caption = "Team Leistung im Durchschnitt (sortiert nach durchschnittlich geschossene Tore pro Spiel [Absteigend]): ";
				PrintResult(caption, dto);
				var dtt = unitOfWork.Teams.GetTeamTable();
				caption = "Team Tabelle (sortiert nach Rang): ";
				PrintResult(caption, dtt);
			}
		}
		/// <summary>
		/// Erstellt eine Konsolenausgabe
		/// </summary>
		/// <param name="caption">Enthält die Überschrift</param>
		/// <param name="result">Enthält das ermittelte Ergebnise</param>
		private static void PrintResult(string caption, string result, bool table)
		{
			Console.WriteLine();
			if (!string.IsNullOrEmpty(caption))
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(new String('=', caption.Length));
				Console.WriteLine(caption);
				Console.WriteLine(new String('=', caption.Length));
				Console.ResetColor();
				Console.WriteLine();
			}
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.WriteLine(result);
				Console.ResetColor();
				Console.WriteLine();
		}
		private static void PrintResult<T>(string caption, IEnumerable<T> elements)
		{
			Console.WriteLine();
			if (!string.IsNullOrEmpty(caption))
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(new String('=', caption.Length));
				Console.WriteLine(caption);
				Console.WriteLine(new String('=', caption.Length));
				Console.ResetColor();
				Console.WriteLine();
			}
			Console.ForegroundColor = ConsoleColor.DarkGray;
			ConsoleTable.From(elements).Configure(o => o.NumberAlignment = Alignment.Right).Write(Format.Alternative);
			Console.ResetColor();
			Console.WriteLine();
		}
	}
}
