﻿namespace Smod.PatreonPlugin
{
	class ConfigOptions
	{
		public readonly static string PATREON_ITEMS = "patreon_items";
		public readonly static string PATREON_TAG = "patreon_tag";
		public readonly static string PATREON_TAG_COLOUR = "patreon_tag_colour";
		public readonly static string PATREON_TAG_AUTO_REFRESH = "patreon_tag_auto_refresh";

		public readonly static string RANK_ITEMS = "{0}_items";
		public readonly static string RANK_TAG = "{0}_tag";
		public readonly static string RANK_TAG_COLOUR = "{0}_tag_colour";
		public readonly static string RANK_TAG_AUTO_REFRESH = "{0}_tag_auto_refresh";

		public static bool ContainsRank(string rankConfigString, string rank)
		{
			return !string.IsNullOrEmpty(rank) && ConfigFile.Contains(string.Format(rankConfigString, rank));
		}

		public static string GetRankConfig(string rankConfigString, string rank)
		{
			return string.Format(rankConfigString, rank);
		}
	}
}