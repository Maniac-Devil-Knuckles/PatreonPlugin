using System.Collections.Generic;
using MEC;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;

namespace Dankrushen.PatreonPlugin
{
	public class PlayerJoinHandler : IEventHandlerPlayerJoin
	{
		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			Timing.RunCoroutine(OnPlayerJoinCoroutine(ev.Player));
		}

		public static IEnumerator<float> OnPlayerJoinCoroutine(Player player)
		{
			SetPatronRoles(player);
			ReservedSlotManager.UpdateReservedSlot(player);
			yield break;
		}

		public static void SetPatronRoles(Player[] players)
		{
			if (ArgumentParsing.ArrayIsNullOrEmpty(players))
				return;

			PatreonPlugin.CreateFile();

			Patron[] patronIds = PatreonPlugin.Patrons;

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
			SetPatronRoles(new[] {player});
		}
	}
}