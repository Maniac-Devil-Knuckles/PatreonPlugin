using System.Collections.Generic;
using System.IO;
using System.Linq;
using Smod2.Commands;

namespace Dankrushen.PatreonPlugin
{
	public class PatreonCommand : ICommandHandler
	{
		public string GetCommandDescription()
		{
			return "Adds or removes SteamIDs from the Patreon supporter list";
		}

		public string GetUsage()
		{
			return "PATREON <ADD <FILE LINE> / REMOVE <STEAMID64> / REFRESH>";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			List<string> output = new List<string>();

			PatreonPlugin.CreateFile();

			if (args.Length >= 1)
			{
				switch (args[0].ToLower())
				{
					case "add":
						if (args.Length >= 2)
						{
							string entry = string.Join(" ", args.Skip(1)).Trim();
							output.Add(IsValidEntry(entry) ? AddPatron(entry) : Errors.InvalidSteamId);
						}
						else
						{
							output.Add(Errors.NotEnoughArgs);
						}
						break;

					case "remove":
						if (args.Length >= 2)
						{
							output.Add(IsSteamId(args[1]) ? RemovePatron(args[1].Trim()) : Errors.InvalidSteamId);
						}
						else
						{
							output.Add(Errors.NotEnoughArgs);
						}
						break;

					case "refresh":
						output.Add(RefreshPatrons());
						break;

					default:
						output.Add(Errors.FuncNotFound);
						break;
				}
			}
			else
			{
				output.Add(Errors.NotEnoughArgs);
			}

			return output.ToArray();
		}

		public bool IsValidEntry(string entry)
		{
			return Patron.FromString(entry) != null;
		}

		public bool HasSteamId(string steamId)
		{
			if (string.IsNullOrEmpty(steamId))
				return false;

			foreach (Patron patron in PatreonPlugin.GetPatrons())
			{
				if (steamId.Trim() == patron.SteamId)
				{
					return true;
				}
			}

			return false;
		}

		public bool HasSteamId(Patron patron)
		{
			return patron != null && HasSteamId(patron.SteamId);
		}

		public string AddPatron(string entry)
		{
			string curText = File.ReadAllText(PatreonPlugin.PatronFile);
			if (!string.IsNullOrEmpty(curText.Trim()) && !curText.EndsWith(System.Environment.NewLine))
			{
				File.AppendAllText(PatreonPlugin.PatronFile, System.Environment.NewLine);
			}

			Patron newPatron = Patron.FromString(entry);

			if (newPatron == null)
				return Errors.InvalidSteamId;

			if (HasSteamId(newPatron))
				return Errors.SteamIdOnList;

			if (!string.IsNullOrEmpty(curText.Trim()))
			{
				File.AppendAllText(PatreonPlugin.PatronFile, entry + System.Environment.NewLine);
			}
			else
			{
				File.WriteAllText(PatreonPlugin.PatronFile, entry + System.Environment.NewLine);
			}

			return string.Format(Successes.AddPatron, newPatron.SteamId);
		}

		public string RemovePatron(string steamId)
		{
			if (!HasSteamId(steamId))
				return Errors.SteamIdNotOnList;

			List<string> patronIds = new List<string>(File.ReadAllLines(PatreonPlugin.PatronFile));

			for (int i = 0; i < patronIds.Count; i++)
			{
				Patron patron = Patron.FromString(patronIds[i]);
				if (patron != null && steamId.Trim() == patron.SteamId)
				{
					patronIds.RemoveAt(i);
				}
			}

			File.WriteAllLines(PatreonPlugin.PatronFile, patronIds.ToArray());

			return string.Format(Successes.RemovePatron, steamId);

		}

		public string RefreshPatrons()
		{
			PlayerJoinHandler.SetPatronRoles(ServerMod2.API.SmodPlayer.GetPlayers().ToArray());

			return Successes.RefreshPatron;
		}

		public static bool IsSteamId(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return false;
			}

			input = input.Trim();

			if (input.Length != 17)
			{
				return false;
			}

			foreach (char digit in input)
			{
				if (!char.IsNumber(digit))
				{
					return false;
				}
			}

			return true;
		}
	}
}
