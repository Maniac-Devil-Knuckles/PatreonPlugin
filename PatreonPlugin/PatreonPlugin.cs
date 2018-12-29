using System.Collections.Generic;
using System.IO;
using Smod2;
using Smod2.Attributes;
using Smod2.EventHandlers;

namespace Dankrushen.PatreonPlugin
{
	[PluginDetails(
		author = "Dankrushen",
		name = "PatreonPlugin",
		description = "A plugin to reward Patreon supporters",
		id = "dankrushen.patreon",
		version = "1.9",
		SmodMajor = 3,
		SmodMinor = 2,
		SmodRevision = 0
		)]
	public class PatreonPlugin : Plugin
	{
		public static PatreonPlugin Singleton { get; private set; }
		public static string PatronFile = FileManager.GetAppFolder(true) + "PatronList.txt";

		public override void OnDisable()
		{
			Info(Details.name + " v" + Details.version + " has been disabled!");
		}

		public override void OnEnable()
		{
			CreateFile();

			Singleton = this;

			Info(Details.name + " v" + Details.version + " has been enabled!");
		}

		public override void Register()
		{
			// Register Events
			AddEventHandler(typeof(IEventHandlerSetRole), new ClassSetHandler());
			AddEventHandler(typeof(IEventHandlerPlayerJoin), new PlayerJoinHandler());
			// Register config settings
			AddConfig(new Smod2.Config.ConfigSetting(ConfigOptions.PatreonItems, "", Smod2.Config.SettingType.STRING, true, "The items to give Patreon supporters when they spawn"));
			AddConfig(new Smod2.Config.ConfigSetting(ConfigOptions.PatreonTag, "", Smod2.Config.SettingType.STRING, true, "The tag to give Patreon supporters"));
			AddConfig(new Smod2.Config.ConfigSetting(ConfigOptions.PatreonTagColour, "default", Smod2.Config.SettingType.STRING, true, "The colour of the tag to give to Patreon supporters"));
			AddConfig(new Smod2.Config.ConfigSetting(ConfigOptions.PatreonTagAutoRefresh, false, Smod2.Config.SettingType.BOOL, true, "Whether to automatically refresh Patreon supporters tags every time their class is set"));
			AddConfig(new Smod2.Config.ConfigSetting(ConfigOptions.PatreonAutoReserve, false, Smod2.Config.SettingType.BOOL, true, "Whether to automatically make reserved slots for Patreon supporters"));
			// Register commands
			AddCommand("patreon", new PatreonCommand());
		}

		public static void CreateFile()
		{
			if (!File.Exists(PatronFile))
			{
				File.Create(PatronFile);
			}
		}

		public static List<Patron> GetPatrons()
		{
			List<string> patronIds = new List<string>(File.ReadAllLines(PatronFile));
			List<Patron> patrons = new List<Patron>();

			foreach (string patronId in patronIds)
			{
				Patron patron = Patron.FromString(patronId);

				if (patron != null)
				{
					patrons.Add(patron);
				}
			}

			return patrons;
		}
	}
}
