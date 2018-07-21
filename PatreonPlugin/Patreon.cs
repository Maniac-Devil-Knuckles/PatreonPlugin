using System;
using System.Text.RegularExpressions;

namespace Smod.PatreonPlugin
{
	class Patreon
	{
		public string SteamId { get; }
		public string Tag { get; }
		public string Colour { get; }
		public string Items { get; }
		public bool AutoRefresh { get; }
		public string Rank { get; }

		public Patreon(string steamId, string tag = null, string colour = null, string items = null, bool autoRefresh = false, string rank = null)
		{
			SteamId = steamId;
			Tag = tag;
			Colour = colour;
			Items = items;
			AutoRefresh = autoRefresh;
			Rank = rank;
		}

		private static readonly string optionRegex = "(\\s|^)({0})(\\s?=\\s?|\\s)(\"[^\"]+\"|[^\\s\"]+)";
		private static readonly string optionOptionRegex = "\\s*({0})(\\s?=\\s?|\\s)";

		private static readonly string tagOnlyRegex = "(\\s|^)({0})(?=\\s|$)"; // For example: "SteamID -a -c "blue""

		private static readonly string tagTags = "-t|--tag|--customtag|--custom";
		private static readonly Regex tagRegex = new Regex(String.Format(optionRegex, tagTags));
		private static readonly Regex tagOptionRegex = new Regex(String.Format(optionOptionRegex, tagTags));

		private static readonly string colourTags = "-c|--colour|--color";
		private static readonly Regex colourRegex = new Regex(String.Format(optionRegex, colourTags));
		private static readonly Regex colourOptionRegex = new Regex(String.Format(optionOptionRegex, colourTags));

		private static readonly string itemTags = "-i|--items|--customitems";
		private static readonly Regex itemsRegex = new Regex(String.Format(optionRegex, itemTags));
		private static readonly Regex itemsOptionRegex = new Regex(String.Format(optionOptionRegex, itemTags));

		private static readonly string autoRefreshTags = "-a|--auto|--autorefresh";
		private static readonly Regex autoRefreshRegex = new Regex(String.Format(tagOnlyRegex, itemTags));

		private static readonly string rankTags = "-r|--rank";
		private static readonly Regex rankRegex = new Regex(String.Format(optionRegex, rankTags));
		private static readonly Regex rankOptionRegex = new Regex(String.Format(optionOptionRegex, rankTags));

		public static Patreon FromString(string fileLine)
		{
			if (fileLine != null)
			{
				fileLine = fileLine.Trim();
			}

			if (string.IsNullOrEmpty(fileLine))
			{
				return null;
			}

			string tagTagValue = MatchOption(fileLine, tagRegex, tagOptionRegex);
			string colourTagValue = MatchOption(fileLine, colourRegex, colourOptionRegex);
			string itemTagValue = MatchOption(fileLine, itemsRegex, itemsOptionRegex);
			bool autoRefreshTags = MatchFlag(fileLine, autoRefreshRegex);
			string rankTagValue = MatchOption(fileLine, rankRegex, rankOptionRegex);

			string steam64Match = FindFirstSteamID(fileLine);
			if (!string.IsNullOrEmpty(steam64Match))
			{
				// Set from default or rank values
				if (tagTagValue == null)
				{
					string rankConf = ConfigOptions.RANK_TAG;
					string defConf = ConfigOptions.PATREON_TAG;
					tagTagValue = ConfigOptions.ContainsRank(rankConf, rankTagValue) ? ConfigFile.GetString(ConfigOptions.GetRankConf(rankConf, rankTagValue)) : PatreonPlugin.singleton.GetConfigString(defConf);

					PatreonPlugin.singleton.Debug("No custom tag value, using default tag from " + (ConfigOptions.ContainsRank(rankConf, rankTagValue) ? "rank" : "patreon") + " config: \"" + tagTagValue + "\"");
				}

				if (colourTagValue == null)
				{
					string rankConf = ConfigOptions.RANK_TAG_COLOUR;
					string defConf = ConfigOptions.PATREON_TAG_COLOUR;
					colourTagValue = ConfigOptions.ContainsRank(rankConf, rankTagValue) ? ConfigFile.GetString(ConfigOptions.GetRankConf(rankConf, rankTagValue)) : PatreonPlugin.singleton.GetConfigString(defConf);

					PatreonPlugin.singleton.Debug("No custom colour value, using default colour from " + (ConfigOptions.ContainsRank(rankConf, rankTagValue) ? "rank" : "patreon") + " config: \"" + colourTagValue + "\"");
				}

				if (itemTagValue == null)
				{
					string rankConf = ConfigOptions.RANK_ITEMS;
					string defConf = ConfigOptions.PATREON_ITEMS;
					itemTagValue = ConfigOptions.ContainsRank(rankConf, rankTagValue) ? ConfigFile.GetString(ConfigOptions.GetRankConf(rankConf, rankTagValue)) : PatreonPlugin.singleton.GetConfigString(defConf);

					PatreonPlugin.singleton.Debug("No custom items value, using default items from " + (ConfigOptions.ContainsRank(rankConf, rankTagValue) ? "rank" : "patreon") + " config: \"" + itemTagValue + "\"");
				}

				if (!autoRefreshTags)
				{
					string rankConf = ConfigOptions.RANK_TAG_AUTO_REFRESH;
					string defConf = ConfigOptions.PATREON_TAG_AUTO_REFRESH;
					autoRefreshTags = ConfigOptions.ContainsRank(rankConf, rankTagValue) ? ConfigFile.GetBool(ConfigOptions.GetRankConf(rankConf, rankTagValue)) : PatreonPlugin.singleton.GetConfigBool(defConf);

					PatreonPlugin.singleton.Debug("No custom autorefresh value, using default autorefresh from " + (ConfigOptions.ContainsRank(rankConf, rankTagValue) ? "rank" : "patreon") + " config: \"" + autoRefreshTags + "\"");
				}

				// Return new instance with values
				return new Patreon(steam64Match, tagTagValue, colourTagValue, itemTagValue, autoRefreshTags, rankTagValue);
			}
			else
			{
				return null;
			}
		}

		public static string MatchOption(string fileLine, Regex tagRegex, Regex optionRegex)
		{
			Match tagMatch = tagRegex.Match(fileLine);
			if (tagMatch.Success && !string.IsNullOrEmpty(tagMatch.Value.Trim()))
			{
				Match optionMatch = optionRegex.Match(tagMatch.Value.Trim());

				if (optionMatch.Success && !string.IsNullOrEmpty(optionMatch.Value.Trim()))
				{
					string finalOption = tagMatch.Value.Trim().Remove(0, optionMatch.Value.Length).Trim();

					if (finalOption.StartsWith("\"") && finalOption.EndsWith("\""))
					{
						finalOption = finalOption.Substring(1, finalOption.Length - 2);
					}

					return finalOption;
				}
			}

			return null;
		}

		public static bool MatchFlag(string fileLine, Regex flagRegex)
		{
			Match flagMatch = flagRegex.Match(fileLine);
			return flagMatch.Success;
		}

		public static string FindFirstSteamID(string fileLine)
		{
			if (string.IsNullOrEmpty(fileLine) || fileLine.Length < 17)
			{
				return null;
			}

			for (int i = 0; i <= fileLine.Length - 17; i++)
			{
				string result = fileLine.Substring(i, 17);

				if (PatreonCommand.IsSteamID(result))
				{
					return result;
				}
			}

			return null;
		}
	}
}
