using JetBrains.Annotations;

namespace Dankrushen.PatreonPlugin
{
	public class Patron
	{
		public string SteamId { get; }
		public string Tag { get; }
		public string Colour { get; }
		public string Items { get; }
		public bool AutoRefresh { get; }
		public bool AutoReserve { get; }
		public string Rank { get; }

		public Patron(string steamId, string tag = null, string colour = null, string items = null, bool autoRefresh = false, bool autoReserve = false, string rank = null)
		{
			SteamId = steamId;
			Tag = tag;
			Colour = colour;
			Items = items;
			AutoRefresh = autoRefresh;
			AutoReserve = autoReserve;
			Rank = rank;
		}

		private static readonly string[] tagKeys =
		{
			"tag",
			"customtag",
			"custom"
		};

		private static readonly string[] tagAliases =
		{
			"t"
		};

		private static readonly string[] colourKeys =
		{
			"colour",
			"color"
		};

		private static readonly string[] colourAliases =
		{
			"c"
		};

		private static readonly string[] itemKeys =
		{
			"items",
			"customitems"
		};

		private static readonly string[] itemAliases =
		{
			"i"
		};

		private static readonly string[] autoRefreshKeys =
		{
			"auto",
			"autorefresh"
		};

		private static readonly string[] autoRefreshAliases =
		{
			"a"
		};

		private static readonly string[] autoReserveKeys =
		{
			"slot",
			"reserved",
			"reservedslot"
		};

		private static readonly string[] autoReserveAliases =
		{
			"s"
		};

		private static readonly string[] rankKeys =
		{
			"rank"
		};

		private static readonly string[] rankAliases =
		{
			"r"
		};

		[CanBeNull]
		public static Patron FromString(string fileLine)
		{
			fileLine = fileLine?.Trim();

			if (string.IsNullOrEmpty(fileLine))
				return null;

			string[] args = ArgumentParsing.StringToArgs(fileLine);

			if (ArgumentParsing.ArrayIsNullOrEmpty(args))
				return null;

			string steam64Match = args[0];

			string tagTagValue = ArgumentParsing.GetParamFromArgs(args, tagKeys, tagAliases);
			string colourTagValue = ArgumentParsing.GetParamFromArgs(args, colourKeys, colourAliases);
			string itemTagValue = ArgumentParsing.GetParamFromArgs(args, itemKeys, itemAliases);
			bool autoRefresh = ArgumentParsing.GetFlagFromArgs(args, autoRefreshKeys, autoRefreshAliases);
			bool autoReserve = ArgumentParsing.GetFlagFromArgs(args, autoReserveKeys, autoReserveAliases);
			string rankTagValue = ArgumentParsing.GetParamFromArgs(args, rankKeys, rankAliases);

			// Set from default or rank values
			if (tagTagValue == null)
			{
				const string rankConf = ConfigOptions.RankTag;
				const string defConf = ConfigOptions.PatreonTag;
				tagTagValue = ConfigOptions.ContainsRank(rankConf, rankTagValue) ? ConfigFile.GetString(ConfigOptions.GetRankConf(rankConf, rankTagValue)) : PatreonPlugin.Singleton.GetConfigString(defConf);

				// PatreonPlugin.Singleton.Debug("No custom tag value, using default tag from " + (ConfigOptions.ContainsRank(rankConf, rankTagValue) ? "rank" : "patreon") + " config: \"" + tagTagValue + "\"");
			}

			if (colourTagValue == null)
			{
				const string rankConf = ConfigOptions.RankTagColour;
				const string defConf = ConfigOptions.PatreonTagColour;
				colourTagValue = ConfigOptions.ContainsRank(rankConf, rankTagValue) ? ConfigFile.GetString(ConfigOptions.GetRankConf(rankConf, rankTagValue)) : PatreonPlugin.Singleton.GetConfigString(defConf);

				// PatreonPlugin.Singleton.Debug("No custom colour value, using default colour from " + (ConfigOptions.ContainsRank(rankConf, rankTagValue) ? "rank" : "patreon") + " config: \"" + colourTagValue + "\"");
			}

			if (itemTagValue == null)
			{
				const string rankConf = ConfigOptions.RankItems;
				const string defConf = ConfigOptions.PatreonItems;
				itemTagValue = ConfigOptions.ContainsRank(rankConf, rankTagValue) ? ConfigFile.GetString(ConfigOptions.GetRankConf(rankConf, rankTagValue)) : PatreonPlugin.Singleton.GetConfigString(defConf);

				// PatreonPlugin.Singleton.Debug("No custom items value, using default items from " + (ConfigOptions.ContainsRank(rankConf, rankTagValue) ? "rank" : "patreon") + " config: \"" + itemTagValue + "\"");
			}

			if (!autoRefresh)
			{
				const string rankConf = ConfigOptions.RankTagAutoRefresh;
				const string defConf = ConfigOptions.PatreonTagAutoRefresh;
				autoRefresh = ConfigOptions.ContainsRank(rankConf, rankTagValue) ? ConfigFile.GetBool(ConfigOptions.GetRankConf(rankConf, rankTagValue)) : PatreonPlugin.Singleton.GetConfigBool(defConf);

				// PatreonPlugin.Singleton.Debug("No custom auto-refresh value, using default auto-refresh from " + (ConfigOptions.ContainsRank(rankConf, rankTagValue) ? "rank" : "patreon") + " config: \"" + AutoRefreshTags + "\"");
			}

			if (!autoReserve)
			{
				const string rankConf = ConfigOptions.RankAutoReserve;
				const string defConf = ConfigOptions.PatreonAutoReserve;
				autoReserve = ConfigOptions.ContainsRank(rankConf, rankTagValue) ? ConfigFile.GetBool(ConfigOptions.GetRankConf(rankConf, rankTagValue)) : PatreonPlugin.Singleton.GetConfigBool(defConf);

				// PatreonPlugin.Singleton.Debug("No custom auto-reserve value, using default auto-reserve from " + (ConfigOptions.ContainsRank(rankConf, rankTagValue) ? "rank" : "patreon") + " config: \"" + AutoRefreshTags + "\"");
			}

			// Return new instance with values
			return new Patron(steam64Match, tagTagValue, colourTagValue, itemTagValue, autoRefresh, autoReserve, rankTagValue);
		}

		public static bool operator ==(Patron a, Patron b)
		{
			if (ReferenceEquals(a, b))
				return true;

			if (a is null || b is null)
				return false;

			return a.SteamId == b.SteamId && a.Tag == b.Tag && a.Colour == b.Colour && a.Items == b.Items && a.AutoRefresh == b.AutoRefresh && a.AutoReserve == b.AutoReserve && a.Rank == b.Rank;
		}

		public static bool operator !=(Patron a, Patron b)
		{
			return !(a == b);
		}
	}
}