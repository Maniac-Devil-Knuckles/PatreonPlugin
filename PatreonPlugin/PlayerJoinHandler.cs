using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System.Collections.Generic;

namespace Smod.PatreonPlugin
{
	class PlayerJoinHandler : IEventHandlerPlayerJoin
	{
		private PatreonPlugin plugin;

		public PlayerJoinHandler(PatreonPlugin plugin)
		{
			this.plugin = plugin;
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
					if (!string.IsNullOrEmpty(player.SteamId) && patreon.SteamId.Equals(player.SteamId))
					{
						player.SetRank(string.IsNullOrEmpty(patreon.CustomTag) ? plugin.GetConfigString("patreon_tag_colour") : patreon.CustomTag, string.IsNullOrEmpty(patreon.CustomColour) ? plugin.GetConfigString("patreon_tag") : patreon.CustomColour);
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
