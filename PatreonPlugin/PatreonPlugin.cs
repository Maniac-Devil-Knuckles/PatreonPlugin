using Smod2;
using Smod2.Attributes;
using Smod2.EventHandlers;
using Smod2.Events;
using System.Collections.Generic;
using System.IO;

namespace Smod.PatreonPlugin
{
	[PluginDetails(
		author = "Dankrushen",
		name = "PatreonPlugin",
		description = "A plugin to reward Patreon supporters",
		id = "dankrushen.patreon",
		version = "1.4",
		SmodMajor = 3,
		SmodMinor = 1,
		SmodRevision = 0
		)]
	class PatreonPlugin : Plugin
	{
		public static readonly string patreonFile = "PatreonList.txt";

		public override void OnDisable()
		{
			this.Info(Details.name + " v" + Details.version + " has been disabled!");
		}

		public override void OnEnable()
		{
			CreateFile();

			this.Info(Details.name + " v" + Details.version + " has been enabled!");
		}

		public override void Register()
		{
			// Register Events
			this.AddEventHandler(typeof(IEventHandlerSetRole), new ClassSetHandler(this), Priority.Normal);
			this.AddEventHandler(typeof(IEventHandlerPlayerJoin), new PlayerJoinHandler(this), Priority.Normal);
			// Register config settings
			this.AddConfig(new Smod2.Config.ConfigSetting("patreon_items", new string[] { }, Smod2.Config.SettingType.LIST, true, "The items to give Patreon supporters when they spawn"));
			this.AddConfig(new Smod2.Config.ConfigSetting("patreon_tag", "", Smod2.Config.SettingType.STRING, true, "The tag to give Patreon supporters"));
			this.AddConfig(new Smod2.Config.ConfigSetting("patreon_tag_colour", "default", Smod2.Config.SettingType.STRING, true, "The colour of the tag to give to Patreon supporters"));
			// Register commands
			this.AddCommand("patreon", new PatreonCommand(this));
		}

		public static void CreateFile()
		{
			if (!File.Exists(patreonFile))
			{
				File.Create(patreonFile);
			}
		}

		public static List<Patreon> GetPatreons()
		{
			List<string> patreonIDs = new List<string>(File.ReadAllLines(PatreonPlugin.patreonFile));
			List<Patreon> patreons = new List<Patreon>();

			foreach (string patreonID in patreonIDs)
			{
				Patreon patreon = Patreon.FromString(patreonID);

				if (patreon != null)
				{
					patreons.Add(patreon);
				}
			}

			return patreons;
		}
	}
}
