using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System.Collections.Generic;

namespace Smod.PatreonPlugin
{
	class PlayerJoinHandler : IEventHandlerPlayerJoin
	{
		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			SetPatreonRoles(ev.Player);
		}

		public static void SetPatreonRoles(Player[] players)
		{
			PatreonPlugin plugin = PatreonPlugin.singleton;

			PatreonPlugin.CreateFile();

			List<Patreon> patreonIDs = PatreonPlugin.GetPatreons();

			foreach (Player player in players)
			{
				foreach (Patreon patreon in patreonIDs)
				{
					if (!string.IsNullOrEmpty(player.SteamId) && patreon.SteamId == player.SteamId)
					{
						player.SetRank(color: patreon.Colour, text: patreon.Tag);
						break;
					}
				}
			}
		}

		public static void SetPatreonRoles(Player player)
		{
			SetPatreonRoles(new Player[] { player });
		}
	}
}
