using System.Collections.Generic;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;

namespace Dankrushen.PatreonPlugin
{
	public class PlayerJoinHandler : IEventHandlerPlayerJoin
	{
		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			SetPatronRoles(ev.Player);
			ReservedSlotManager.UpdateReservedSlot(ev.Player);
		}

		public static void SetPatronRoles(Player[] players)
		{
			PatreonPlugin.CreateFile();

			List<Patron> patronIds = PatreonPlugin.GetPatrons();

			foreach (Player player in players)
			{
				foreach (Patron patron in patronIds)
				{
					if (string.IsNullOrEmpty(player.SteamId) || patron.SteamId != player.SteamId) continue;

					player.SetRank(patron.Colour, patron.Tag);
					break;
				}
			}
		}

		public static void SetPatronRoles(Player player)
		{
			SetPatronRoles(new[] { player });
		}
	}
}
