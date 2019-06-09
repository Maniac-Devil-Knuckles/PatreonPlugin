using System.IO;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Config;

namespace Dankrushen.PatreonPlugin
{
	[PluginDetails(
		author = "Dankrushen",
		name = "PatreonPlugin",
		description = "A plugin to reward Patreon supporters",
		id = "dankrushen.patreon",
		configPrefix = "patreon",
		version = "2.0",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 1
	)]
	public class PatreonPlugin : Plugin
	{
		public static PatreonPlugin Singleton { get; private set; }

		public const string PatronFileName = "PatronList.txt";
		public static readonly string PatronFile = FileManager.GetAppFolder(true) + PatronFileName;

		private static FileSystemWatcher patronFileWatcher;

		public static Patron[] FilePatrons => File.ReadAllLines(PatronFile).Select(Patron.FromString).Where(patron => patron != null).ToArray();

		private static readonly object patronsLock = new object();
		private static Patron[] patrons;

		public static Patron[] Patrons
		{
			get
			{
				lock (patronsLock)
				{
					return patrons ?? UpdateCachedPatrons();
				}
			}
		}

		public static Player[] Players => PluginManager.Manager?.Server?.GetPlayers()?.ToArray() ?? new Player[0];

		public override void OnDisable()
		{
			patronFileWatcher.Dispose();
			patronFileWatcher = null;

			Info(Details.name + " v" + Details.version + " has been disabled!");
		}

		public override void OnEnable()
		{
			Singleton = this;

			CreateFile();

			patronFileWatcher = new FileSystemWatcher {Path = FileManager.GetAppFolder(true), Filter = PatronFileName, NotifyFilter = NotifyFilters.LastWrite};
			patronFileWatcher.Changed += (sender, args) => { UpdateCachedPatrons(); };
			patronFileWatcher.EnableRaisingEvents = true;

			Info(Details.name + " v" + Details.version + " has been enabled!");
		}

		public override void Register()
		{
			// Register Events
			AddEventHandlers(new ClassSetHandler());
			AddEventHandlers(new PlayerJoinHandler());

			// Register config settings
			AddConfig(new ConfigSetting(ConfigOptions.PatreonItems, "", true, "The items to give Patreon supporters when they spawn"));
			AddConfig(new ConfigSetting(ConfigOptions.PatreonTag, "", true, "The tag to give Patreon supporters"));
			AddConfig(new ConfigSetting(ConfigOptions.PatreonTagColour, "default", true, "The colour of the tag to give to Patreon supporters"));
			AddConfig(new ConfigSetting(ConfigOptions.PatreonTagAutoRefresh, false, true, "Whether to automatically refresh Patreon supporters tags every time their class is set"));
			AddConfig(new ConfigSetting(ConfigOptions.PatreonAutoReserve, false, true, "Whether to automatically make reserved slots for Patreon supporters"));

			// Register commands
			AddCommand("patreon", new PatreonCommand());
		}

		public static bool CreateFile()
		{
			if (File.Exists(PatronFile)) return false;

			File.Create(PatronFile).Close();
			return true;
		}

		public static Patron[] UpdateCachedPatrons()
		{
			Singleton.Debug("Updating array of cached patrons...");

			lock (patronsLock)
			{
				Patron[] filePatrons = CreateFile() ? new Patron[0] : FilePatrons;
				return patrons = filePatrons;
			}
		}
	}
}