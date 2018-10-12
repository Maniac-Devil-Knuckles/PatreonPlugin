using System.Collections.Generic;
using Smod2.API;

namespace Smod.PatreonPlugin
{
	static class ReservedSlotManager
	{
		public readonly static string SLOT_COMMENT_PREFIX = "<PATREON>";

		public static ReservedSlot[] GetPatreonSlots()
		{
			List<ReservedSlot> patreonSlots = new List<ReservedSlot>();

			foreach (ReservedSlot slot in ReservedSlot.GetSlots())
			{
				if (!string.IsNullOrEmpty(slot.Comment) && slot.Comment.Trim().StartsWith(SLOT_COMMENT_PREFIX))
				{
					patreonSlots.Add(slot);
				}
			}

			return patreonSlots.ToArray();
		}

		public static bool IsValidPatreon(string steamID)
		{
			foreach (Patreon patron in PatreonPlugin.GetPatreons())
			{
				if (patron.SteamId == steamID.Trim() && patron.AutoReserve)
				{
					return true;
				}
			}

			return false;
		}

		public static bool ReservedSlotsContains(string steamID)
		{
			foreach (ReservedSlot slot in ReservedSlot.GetSlots())
			{
				if (slot.SteamID == steamID.Trim())
				{
					return true;
				}
			}

			return false;
		}

		public static void UpdateReservedSlot(Player player)
		{
			// Remove Patreons that are no longer in the list
			foreach (ReservedSlot slot in GetPatreonSlots())
			{
				if (!IsValidPatreon(slot.SteamID))
				{
					slot.RemoveSlotFromFile();
				}
			}

			// Add Patron to reserved slot if they aren't already
			foreach (Patreon patron in PatreonPlugin.GetPatreons())
			{
				if (patron.AutoReserve && patron.SteamId == player.SteamId && !ReservedSlotsContains(player.SteamId))
				{
					// Make reserved slot for patron and append it to the Reserved Slot file
					new ReservedSlot(player.IpAddress, player.SteamId, SLOT_COMMENT_PREFIX + " " + player.Name).AppendToFile();
				}
			}
		}
	}
}
