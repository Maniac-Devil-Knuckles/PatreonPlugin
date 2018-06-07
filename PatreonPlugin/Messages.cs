namespace Smod.PatreonPlugin
{
	static class Errors
	{
		public const string NotEnoughArgs = "Error: Not enough arguments!";
		public const string FuncNotFound = "Error: Function not found!";
		public const string InvalidSteamId = "Error: Invalid SteamID!";

		public const string SteamIdOnList = "Error: This SteamID is already on the Patreons list!";
		public const string SteamIdNotOnList = "Error: This SteamID is not on the Patreons list!";

		public const string IntegerParse = "Error: Integer parsing failed in \"{0}\" from the config option \"patreon_items\"!";
		public const string MissingSplitChar = "Error: Missing split character in \"{0}\" from the config option \"patreon_items\"!";
	}

	static class Successes
	{
		public const string AddPatreon = "Successfully added \"{0}\" to the Patreons list!";
		public const string RemovePatreon = "Successfully removed \"{0}\" from the Patreons list!";
		public const string RefreshPatreon = "Successfully refreshed the Patreon roles in-game!";
	}
}
