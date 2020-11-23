using GameCore;

namespace Dankrushen.PatreonPlugin
{
	public static class ConfigOptions
	{
		public const string PatreonItems = "patreon_items";
		public const string PatreonTag = "patreon_tag";
		public const string PatreonTagColour = "patreon_tag_colour";
		public const string PatreonTagAutoRefresh = "patreon_tag_auto_refresh";
		public const string PatreonAutoReserve = "patreon_auto_reserve";

		public const string RankItems = "patreon_{0}_items";
		public const string RankTag = "patreon_{0}_tag";
		public const string RankTagColour = "patreon_{0}_tag_colour";
		public const string RankTagAutoRefresh = "patreon_{0}_tag_auto_refresh";
		public const string RankAutoReserve = "patreon_{0}_auto_reserve";

		public static bool ContainsRank(string rankConfigString, string rank)
		{
			return !string.IsNullOrEmpty(rank) && ConfigFile.GetString(string.Format(rankConfigString, rank), null) != null;
		}

		public static string GetRankConf(string rankConfigString, string rank)
		{
			return string.Format(rankConfigString, rank);
		}
	}
}