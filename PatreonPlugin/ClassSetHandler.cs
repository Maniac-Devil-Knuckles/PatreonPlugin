using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;

namespace Smod.PatreonPlugin
{
	class ClassSetHandler : IEventHandlerSetRole
	{
		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			SetPatreonItems(ev.Player, ev.TeamRole);

			foreach (Patreon patreon in PatreonPlugin.GetPatreons())
			{
				if (patreon.SteamId == ev.Player.SteamId && patreon.AutoRefresh)
				{
					PlayerJoinHandler.SetPatreonRoles(ev.Player);
					break;
				}
			}
		}

		public void SetPatreonItems(Player player, TeamRole teamRole)
		{
			PatreonPlugin.CreateFile();

			foreach (Patreon patreon in PatreonPlugin.GetPatreons())
			{
				if (patreon.SteamId == player.SteamId)
				{
					foreach (string item in patreon.Items.Split(','))
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
							PatreonPlugin.singleton.Error(string.Format(Errors.IntegerParse, item));
						}
						else
						{
							PatreonPlugin.singleton.Error(string.Format(Errors.MissingSplitChar, item));
						}
					}
				}
			}
		}
	}
}
