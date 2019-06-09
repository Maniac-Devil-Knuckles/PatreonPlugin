using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dankrushen.PatreonPlugin
{
	public static class ArgumentParsing
	{
		public static int IndexOfNonEscaped(string inString, char inChar, char escapeChar = '\\', int startIndex = 0)
		{
			if (!string.IsNullOrEmpty(inString))
			{
				bool escaped = false;

				for (int i = startIndex; i < inString.Length; i++)
				{
					char stringChar = inString[i];

					if (!escaped)
					{
						if (stringChar == escapeChar)
						{
							escaped = true;
							continue;
						}
					}

					// If the character is escaped or the character that's escaped is an escape character then check if it matches
					if ((!escaped || stringChar == escapeChar) && stringChar == inChar)
					{
						return i;
					}

					escaped = false;
				}
			}

			return -1;
		}

		public static string[] StringToArgs(string inString, char separator = ' ', char escapeChar = '\\', char quoteChar = '\"', bool keepQuotes = false)
		{
			if (string.IsNullOrEmpty(inString))
				return new string[0];

			List<string> args = new List<string>();
			StringBuilder strBuilder = new StringBuilder();
			bool inQuotes = false;
			bool escaped = false;

			for (int i = 0; i < inString.Length; i++)
			{
				char stringChar = inString[i];

				if (!escaped)
				{
					if (stringChar == escapeChar)
					{
						escaped = true;
						continue;
					}

					if (stringChar == quoteChar && (inQuotes || IndexOfNonEscaped(inString, quoteChar, escapeChar, i + 1) > 0))
					{
						// Ignore quotes if there's no future non-escaped quotes

						inQuotes = !inQuotes;
						if (!keepQuotes)
							continue;
					}
					else if (!inQuotes && stringChar == separator)
					{
						args.Add(strBuilder.ToString());
						strBuilder.Clear();
						continue;
					}
				}

				strBuilder.Append(stringChar);
				escaped = false;
			}

			args.Add(strBuilder.ToString());

			return args.ToArray();
		}

		public static bool ArrayIsNullOrEmpty(ICollection<object> array)
		{
			return array == null || !array.Any();
		}

		public static string GetParamFromArgs(string[] args, string[] keys = null, string[] aliases = null)
		{
			if (ArrayIsNullOrEmpty(args) || ArrayIsNullOrEmpty(keys) && ArrayIsNullOrEmpty(aliases)) return null;

			for (int i = 0; i < args.Length - 1; i++)
			{
				string lowArg = args[i]?.ToLower();

				if (string.IsNullOrEmpty(lowArg)) continue;

				if (!ArrayIsNullOrEmpty(keys))
				{
					if (keys.Any(key => !string.IsNullOrEmpty(key) && lowArg == $"--{key.ToLower()}"))
					{
						return args[i + 1];
					}
				}

				if (!ArrayIsNullOrEmpty(aliases))
				{
					if (aliases.Any(alias => !string.IsNullOrEmpty(alias) && lowArg == $"-{alias.ToLower()}"))
					{
						return args[i + 1];
					}
				}
			}

			return null;
		}

		public static bool ArgsContainsParam(string[] args, string[] keys = null, string[] aliases = null)
		{
			if (ArrayIsNullOrEmpty(args))
				return false;

			foreach (string arg in args)
			{
				string lowArg = arg?.ToLower();

				if (string.IsNullOrEmpty(lowArg)) continue;

				if (!ArrayIsNullOrEmpty(keys))
				{
					if (keys.Any(key => !string.IsNullOrEmpty(key) && lowArg == $"--{key.ToLower()}"))
					{
						return true;
					}
				}

				if (!ArrayIsNullOrEmpty(aliases))
				{
					if (aliases.Any(alias => !string.IsNullOrEmpty(alias) && lowArg == $"-{alias.ToLower()}"))
					{
						return true;
					}
				}
			}

			return false;
		}

		public static bool GetFlagFromArgs(string[] args, string[] keys = null, string[] aliases = null)
		{
			if (ArrayIsNullOrEmpty(args) || ArrayIsNullOrEmpty(keys) && ArrayIsNullOrEmpty(aliases)) return false;

			return bool.TryParse(GetParamFromArgs(args, keys, aliases), out bool result) ? result : ArgsContainsParam(args, keys, aliases);
		}

		public static string GetParamFromArgs(string[] args, string key = null, string alias = null)
		{
			return GetParamFromArgs(args, new string[] {key}, new string[] {alias});
		}

		public static bool ArgsContainsParam(string[] args, string key = null, string alias = null)
		{
			return ArgsContainsParam(args, new string[] {key}, new string[] {alias});
		}

		public static bool GetFlagFromArgs(string[] args, string key = null, string alias = null)
		{
			return GetFlagFromArgs(args, new string[] {key}, new string[] {alias});
		}
	}
}