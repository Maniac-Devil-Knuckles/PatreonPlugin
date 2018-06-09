using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;

namespace Smod.PatreonPlugin
{
    class ClassSetHandler : IEventHandlerSetRole
	{
		private PatreonPlugin plugin;

		public ClassSetHandler(PatreonPlugin plugin)
		{
			this.plugin = plugin;
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			SetPatreonItems(ev.Player, ev.TeamRole);
			//PlayerJoinHandler.SetPatreonRoles(ev.Player, plugin); // For Blizzard
		}

		public void SetPatreonItems(Player player, TeamRole teamRole)
		{
			PatreonPlugin.CreateFile();

			string[] defaultItems = plugin.GetConfigList("patreon_items");

			foreach (Patreon patreon in PatreonPlugin.GetPatreons())
			{
				if (patreon.SteamId.Equals(player.SteamId))
				{
					foreach (string item in (patreon.CustomItems != null ? patreon.CustomItems.Split(',') : defaultItems))
					{
						string[] split = item.Trim().Split(new char[] { ':' }, 2);

						int charClass;
						int classItem;

						if (split.Length >= 2 && int.TryParse(split[0].Trim(), out charClass) && int.TryParse(split[1].Trim(), out classItem))
						{
							if (((int)teamRole.Role == charClass || charClass < 0) && !teamRole.Team.Equals(Team.RIP) && !teamRole.Team.Equals(Team.SCP))
							{
								player.GiveItem((ItemType)classItem);
							}
						}
						else if (split.Length >= 2)
						{
							plugin.Error(string.Format(Errors.IntegerParse, item));
						}
						else
						{
							plugin.Error(string.Format(Errors.MissingSplitChar, item));
						}
					}
				}
			}
		}
	}
}
