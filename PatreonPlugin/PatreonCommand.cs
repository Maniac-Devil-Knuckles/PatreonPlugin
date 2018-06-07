using Smod2.Commands;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Smod.PatreonPlugin
{
	class PatreonCommand : ICommandHandler
	{
		private PatreonPlugin plugin;

		public PatreonCommand(PatreonPlugin plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Adds or removes SteamIDs from the Patreon list";
		}

		public string GetUsage()
		{
			return "PATREON <ADD / REMOVE / REFRESH> [SteamID]";
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
							if (IsSteamID(args[1]))
							{
								output.Add(AddPatreon(args[1].Trim()));
							}
							else
							{
								output.Add(Errors.InvalidSteamId);
							}
						}
						else
						{
							output.Add(Errors.NotEnoughArgs);
						}
						break;

					case "remove":
						if (args.Length >= 2)
						{
							if (IsSteamID(args[1]))
							{
								output.Add(RemovePatreon(args[1].Trim()));
							}
							else
							{
								output.Add(Errors.InvalidSteamId);
							}
						}
						else
						{
							output.Add(Errors.NotEnoughArgs);
						}
						break;

					case "refresh":
						output.Add(RefreshPatreons());
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

		public bool HasPatreon(string steamID)
		{
			foreach (Patreon patreon in PatreonPlugin.GetPatreons())
			{
				if (steamID.Trim().Equals(patreon.SteamId))
				{
					return true;
				}
			}

			return false;
		}

		public string AddPatreon(string steamID)
		{
			string curText = File.ReadAllText(PatreonPlugin.patreonFile);
			if (!string.IsNullOrEmpty(curText.Trim()) && !curText.EndsWith(System.Environment.NewLine))
			{
				File.AppendAllText(PatreonPlugin.patreonFile, System.Environment.NewLine);
			}

			if (!HasPatreon(steamID))
			{
				if (!string.IsNullOrEmpty(curText.Trim()))
				{
					File.AppendAllText(PatreonPlugin.patreonFile, steamID + System.Environment.NewLine);
				}
				else
				{
					File.WriteAllText(PatreonPlugin.patreonFile, steamID + System.Environment.NewLine);
				}
				return string.Format(Successes.AddPatreon, steamID);
			}
			else
			{
				return Errors.SteamIdOnList;
			}
		}

		public string RemovePatreon(string steamID)
		{
			if (HasPatreon(steamID))
			{
				List<string> patreonIDs = new List<string>(File.ReadAllLines(PatreonPlugin.patreonFile));

				for (int i = 0; i < patreonIDs.Count; i++)
				{
					Patreon patreon = Patreon.FromString(patreonIDs[i]);
					if (patreon != null && steamID.Trim().Equals(patreon.SteamId))
					{
						patreonIDs.RemoveAt(i);
					}
				}

				File.WriteAllLines(PatreonPlugin.patreonFile, patreonIDs.ToArray());

				return string.Format(Successes.RemovePatreon, steamID);
			}
			else
			{
				return Errors.SteamIdNotOnList;
			}
		}

		public string RefreshPatreons()
		{
			PlayerJoinHandler.SetPatreonRoles(ServerMod2.API.SmodPlayer.GetPlayers().ToArray(), plugin);

			return Successes.RefreshPatreon;
		}

		public static bool IsSteamID(string input)
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
