using System.Linq;
using Smod2.API;

namespace Dankrushen.PatreonPlugin
{
	public static class ReservedSlotManager
	{
		public const string SlotCommentPrefix = "<PATREON>";

		public static ReservedSlot[] GetPatronSlots()
		{
			return ReservedSlot.GetSlots().Where(slot => !string.IsNullOrEmpty(slot.Comment) && slot.Comment.Trim().StartsWith(SlotCommentPrefix)).ToArray();
		}

		public static bool IsValidPatron(string steamId)
		{
			foreach (Patron patron in PatreonPlugin.GetPatrons())
				if (patron.SteamId == steamId.Trim() && patron.AutoReserve)
					return true;

			return false;
		}

		public static bool ReservedSlotsContains(string steamId)
		{
			return ReservedSlot.GetSlots().Any(slot => slot.SteamID == steamId.Trim());
		}

		public static void UpdateReservedSlot(Player player)
		{
			// Remove Patrons that are no longer in the list
			foreach (ReservedSlot slot in GetPatronSlots())
				if (!IsValidPatron(slot.SteamID))
					slot.RemoveSlotFromFile();

			// Add Patron to reserved slot if they aren't already
			foreach (Patron patron in PatreonPlugin.GetPatrons())
				if (patron.AutoReserve && patron.SteamId == player.SteamId && !ReservedSlotsContains(player.SteamId))
					new ReservedSlot(player.IpAddress, player.SteamId, SlotCommentPrefix + " " + player.Name).AppendToFile();
		}
	}
}