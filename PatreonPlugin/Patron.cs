using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Dankrushen.PatreonPlugin
{
	public class Patron
	{
		public readonly string SteamId;
		public readonly string Tag;
		public readonly string Colour;
		public readonly string Items;
		public readonly bool AutoRefresh;
		public readonly bool AutoReserve;
		public readonly string Rank;

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

		private const string OptionRegex = "(\\s|^)({0})(\\s?=\\s?|\\s)(\"[^\"]+\"|[^\\s\"]+)";
		private const string OptionOptionRegex = "\\s*({0})(\\s?=\\s?|\\s)";

		private const string TagOnlyRegex = "(\\s|^)({0})(?=\\s|$)"; // For example: "SteamID -a -c "blue""

		private const string TagTags = "-t|--tag|--customtag|--custom";
		private static readonly Regex tagRegex = new Regex(string.Format(OptionRegex, TagTags), RegexOptions.Compiled);
		private static readonly Regex tagOptionRegex = new Regex(string.Format(OptionOptionRegex, TagTags), RegexOptions.Compiled);

		private const string ColourTags = "-c|--colour|--color";
		private static readonly Regex colourRegex = new Regex(string.Format(OptionRegex, ColourTags), RegexOptions.Compiled);
		private static readonly Regex colourOptionRegex = new Regex(string.Format(OptionOptionRegex, ColourTags), RegexOptions.Compiled);

		private const string ItemTags = "-i|--items|--customitems";
		private static readonly Regex itemsRegex = new Regex(string.Format(OptionRegex, ItemTags), RegexOptions.Compiled);
		private static readonly Regex itemsOptionRegex = new Regex(string.Format(OptionOptionRegex, ItemTags), RegexOptions.Compiled);

		private const string AutoRefreshTags = "-a|--auto|--autorefresh";
		private static readonly Regex autoRefreshRegex = new Regex(string.Format(TagOnlyRegex, AutoRefreshTags), RegexOptions.Compiled);

		private const string AutoReserveTags = "-s|--slot|--reserved|--reservedslot";
		private static readonly Regex autoReserveRegex = new Regex(string.Format(TagOnlyRegex, AutoReserveTags), RegexOptions.Compiled);

		private const string RankTags = "-r|--rank";
		private static readonly Regex rankRegex = new Regex(string.Format(OptionRegex, RankTags), RegexOptions.Compiled);
		private static readonly Regex rankOptionRegex = new Regex(string.Format(OptionOptionRegex, RankTags), RegexOptions.Compiled);

		[CanBeNull]
		public static Patron FromString(string fileLine)
		{
			fileLine = fileLine?.Trim();

			if (string.IsNullOrEmpty(fileLine))
				return null;

			string steam64Match = FindFirstSteamId(fileLine);

			if (string.IsNullOrEmpty(steam64Match))
				return null;

			string tagTagValue = MatchOption(fileLine, tagRegex, tagOptionRegex);
			string colourTagValue = MatchOption(fileLine, colourRegex, colourOptionRegex);
			string itemTagValue = MatchOption(fileLine, itemsRegex, itemsOptionRegex);
			bool autoRefresh = MatchFlag(fileLine, autoRefreshRegex);
			bool autoReserve = MatchFlag(fileLine, autoReserveRegex);
			string rankTagValue = MatchOption(fileLine, rankRegex, rankOptionRegex);

			// Set from default or rank values
			if (tagTagValue == null)
			{
				const string rankConf = ConfigOptions.RankTag;
				const string defConf = ConfigOptions.PatreonTag;
				tagTagValue = ConfigOptions.ContainsRank(rankConf, rankTagValue) ? ConfigFile.GetString(ConfigOptions.GetRankConf(rankConf, rankTagValue)) : PatreonPlugin.Singleton.GetConfigString(defConf);

				PatreonPlugin.Singleton.Debug("No custom tag value, using default tag from " + (ConfigOptions.ContainsRank(rankConf, rankTagValue) ? "rank" : "patreon") + " config: \"" + tagTagValue + "\"");
			}

			if (colourTagValue == null)
			{
				const string rankConf = ConfigOptions.RankTagColour;
				const string defConf = ConfigOptions.PatreonTagColour;
				colourTagValue = ConfigOptions.ContainsRank(rankConf, rankTagValue) ? ConfigFile.GetString(ConfigOptions.GetRankConf(rankConf, rankTagValue)) : PatreonPlugin.Singleton.GetConfigString(defConf);

				PatreonPlugin.Singleton.Debug("No custom colour value, using default colour from " + (ConfigOptions.ContainsRank(rankConf, rankTagValue) ? "rank" : "patreon") + " config: \"" + colourTagValue + "\"");
			}

			if (itemTagValue == null)
			{
				const string rankConf = ConfigOptions.RankItems;
				const string defConf = ConfigOptions.PatreonItems;
				itemTagValue = ConfigOptions.ContainsRank(rankConf, rankTagValue) ? ConfigFile.GetString(ConfigOptions.GetRankConf(rankConf, rankTagValue)) : PatreonPlugin.Singleton.GetConfigString(defConf);

				PatreonPlugin.Singleton.Debug("No custom items value, using default items from " + (ConfigOptions.ContainsRank(rankConf, rankTagValue) ? "rank" : "patreon") + " config: \"" + itemTagValue + "\"");
			}

			if (!autoRefresh)
			{
				const string rankConf = ConfigOptions.RankTagAutoRefresh;
				const string defConf = ConfigOptions.PatreonTagAutoRefresh;
				autoRefresh = ConfigOptions.ContainsRank(rankConf, rankTagValue) ? ConfigFile.GetBool(ConfigOptions.GetRankConf(rankConf, rankTagValue)) : PatreonPlugin.Singleton.GetConfigBool(defConf);

				PatreonPlugin.Singleton.Debug("No custom auto-refresh value, using default auto-refresh from " + (ConfigOptions.ContainsRank(rankConf, rankTagValue) ? "rank" : "patreon") + " config: \"" + AutoRefreshTags + "\"");
			}

			if (!autoReserve)
			{
				const string rankConf = ConfigOptions.RankAutoReserve;
				const string defConf = ConfigOptions.PatreonAutoReserve;
				autoReserve = ConfigOptions.ContainsRank(rankConf, rankTagValue) ? ConfigFile.GetBool(ConfigOptions.GetRankConf(rankConf, rankTagValue)) : PatreonPlugin.Singleton.GetConfigBool(defConf);

				PatreonPlugin.Singleton.Debug("No custom auto-reserve value, using default auto-reserve from " + (ConfigOptions.ContainsRank(rankConf, rankTagValue) ? "rank" : "patreon") + " config: \"" + AutoRefreshTags + "\"");
			}

			// Return new instance with values
			return new Patron(steam64Match, tagTagValue, colourTagValue, itemTagValue, autoRefresh, autoReserve, rankTagValue);

		}

		[CanBeNull]
		public static string MatchOption(string fileLine, Regex tagRegex, Regex optionRegex)
		{
			Match tagMatch = tagRegex.Match(fileLine);

			if (!tagMatch.Success || string.IsNullOrEmpty(tagMatch.Value.Trim()))
				return null;

			Match optionMatch = optionRegex.Match(tagMatch.Value.Trim());

			if (!optionMatch.Success || string.IsNullOrEmpty(optionMatch.Value.Trim()))
				return null;

			string finalOption = tagMatch.Value.Trim().Remove(0, optionMatch.Value.Length).Trim();

			if (finalOption.StartsWith("\"") && finalOption.EndsWith("\""))
			{
				finalOption = finalOption.Substring(1, finalOption.Length - 2);
			}

			return finalOption;

		}

		public static bool MatchFlag(string fileLine, Regex flagRegex)
		{
			Match flagMatch = flagRegex.Match(fileLine);
			return flagMatch.Success;
		}

		public static string FindFirstSteamId(string fileLine)
		{
			if (string.IsNullOrEmpty(fileLine) || fileLine.Length < 17)
			{
				return null;
			}

			for (int i = 0; i <= fileLine.Length - 17; i++)
			{
				string result = fileLine.Substring(i, 17);

				if (PatreonCommand.IsSteamId(result))
				{
					return result;
				}
			}

			return null;
		}

		public override bool Equals(object o)
		{
			return ReferenceEquals(this, o);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = (SteamId != null ? SteamId.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Tag != null ? Tag.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Colour != null ? Colour.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Items != null ? Items.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ AutoRefresh.GetHashCode();
				hashCode = (hashCode * 397) ^ AutoReserve.GetHashCode();
				hashCode = (hashCode * 397) ^ (Rank != null ? Rank.GetHashCode() : 0);
				return hashCode;
			}
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
