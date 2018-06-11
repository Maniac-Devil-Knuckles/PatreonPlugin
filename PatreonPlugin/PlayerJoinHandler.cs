using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System.Collections.Generic;

namespace Smod.PatreonPlugin
{
	class PlayerJoinHandler : IEventHandlerPlayerJoin
	{
		private static PatreonPlugin plugin;

		public PlayerJoinHandler(PatreonPlugin plugin)
		{
			PlayerJoinHandler.plugin = plugin;
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			SetPatreonRoles(ev.Player, plugin);
		}

		public static void SetPatreonRoles(Player[] players, PatreonPlugin plugin)
		{
			PatreonPlugin.CreateFile();

			List<Patreon> patreonIDs = PatreonPlugin.GetPatreons();

			foreach (Player player in players)
			{
				foreach (Patreon patreon in patreonIDs)
				{
					if (!string.IsNullOrEmpty(player.SteamId) && patreon.SteamId == player.SteamId)
					{
						string colour = patreon.CustomColour != null && !string.IsNullOrEmpty(patreon.CustomColour.Trim()) ? patreon.CustomColour : plugin.GetConfigString("patreon_tag_colour");
						string tag = patreon.CustomTag != null && !string.IsNullOrEmpty(patreon.CustomTag.Trim()) ? patreon.CustomTag : plugin.GetConfigString("patreon_tag");

						player.SetRank(color: colour, text: tag);
						break;
					}
				}
			}
		}

		public static void SetPatreonRoles(Player player, PatreonPlugin plugin)
		{
			SetPatreonRoles(new Player[] { player }, plugin);
		}
	}
}
