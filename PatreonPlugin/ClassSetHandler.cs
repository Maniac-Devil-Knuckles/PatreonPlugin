﻿using System.Collections.Generic;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;

namespace Dankrushen.PatreonPlugin
{
	public class ClassSetHandler : IEventHandlerSetRole
	{
		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			ev.Items.AddRange(GetPatronItems(ev.Player, ev.TeamRole));

			foreach (Patron patron in PatreonPlugin.Patrons)
			{
				if (patron.UserId != ev.Player.UserId || !patron.AutoRefresh)
					continue;

				PlayerJoinHandler.SetPatronRoles(ev.Player);
				break;
			}
		}

		public static Smod2.API.ItemType[] GetPatronItems(Player player, TeamRole teamRole)
		{
			PatreonPlugin.CreateFile();

			List<Smod2.API.ItemType> items = new List<Smod2.API.ItemType>();

			foreach (Patron patron in PatreonPlugin.Patrons)
			{
				if (patron.UserId != player.UserId || string.IsNullOrEmpty(patron.Items))
					continue;

				foreach (string item in patron.Items.Split(','))
				{
					string[] split = item.Trim().Split(new[] {':'}, 2);

					if (split.Length >= 2 && int.TryParse(split[0].Trim(), out int charClass) && int.TryParse(split[1].Trim(), out int classItem))
					{
						if (((int) teamRole.Role == charClass || charClass < 0) && !teamRole.Team.Equals(Team.RIP) && !teamRole.Team.Equals(Team.SCP))
						{
							items.Add((Smod2.API.ItemType) classItem);
						}
					}
					else if (split.Length >= 2)
					{
						PatreonPlugin.Singleton.Error(string.Format(Errors.IntegerParse, item));
					}
					else
					{
						PatreonPlugin.Singleton.Error(string.Format(Errors.MissingSplitChar, item));
					}
				}
			}

			return items.ToArray();
		}
	}
}