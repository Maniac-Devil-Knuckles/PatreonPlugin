using System;
using System.Text.RegularExpressions;

namespace Smod.PatreonPlugin
{
	class Patreon
	{
		public string SteamId { get; }
		public string CustomTag { get; }
		public string CustomColour { get; }
		public string CustomItems { get; }

		public Patreon(string steamId, string customTag = null, string customColour = null, string customItems = null)
		{
			SteamId = steamId;
			CustomTag = customTag;
			CustomColour = customColour;
			CustomItems = customItems;
		}

		private static readonly string optionRegex = "(\\s|^)({0})(\\s?=\\s?|\\s)(\"[^\"]+\"|[^\\s\"]+)";
		private static readonly string optionOptionRegex = "\\s*({0})(\\s?=\\s?|\\s)";

		private static readonly string tagTags = "-t|--tag|--customtag|--custom";
		private static readonly Regex tagRegex = new Regex(String.Format(optionRegex, tagTags));
		private static readonly Regex tagOptionRegex = new Regex(String.Format(optionOptionRegex, tagTags));

		private static readonly string colourTags = "-c|--colour|--color";
		private static readonly Regex colourRegex = new Regex(String.Format(optionRegex, colourTags));
		private static readonly Regex colourOptionRegex = new Regex(String.Format(optionOptionRegex, colourTags));

		private static readonly string itemTags = "-i|--items|--customitems";
		private static readonly Regex itemsRegex = new Regex(String.Format(optionRegex, itemTags));
		private static readonly Regex itemsOptionRegex = new Regex(String.Format(optionOptionRegex, itemTags));

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

			string steam64Match = FindFirstSteamID(fileLine);
			if (!string.IsNullOrEmpty(steam64Match))
			{
				return new Patreon(steam64Match, tagTagValue, colourTagValue, itemTagValue);
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
