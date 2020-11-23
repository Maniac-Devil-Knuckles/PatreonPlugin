namespace Dankrushen.PatreonPlugin
{
	public static class Errors
	{
		public const string NotEnoughArgs = "Error: Not enough arguments!";
		public const string FuncNotFound = "Error: Function not found!";
		public const string InvalidEntry = "Error: Invalid entry!";

		public const string UserIdOnList = "Error: This UserId is already on the Patreon supporter list!";
		public const string UserIdNotOnList = "Error: This UserId is not on the Patreon supporter list!";

		public const string IntegerParse = "Error: Integer parsing failed in \"{0}\" from the config option \"patreon_items\"!";
		public const string MissingSplitChar = "Error: Missing split character in \"{0}\" from the config option \"patreon_items\"!";
	}

	public static class Successes
	{
		public const string AddPatron = "Successfully added \"{0}\" to the Patreon supporter list!";
		public const string RemovePatron = "Successfully removed \"{0}\" from the Patreon supporter list!";
		public const string RefreshPatron = "Successfully refreshed the Patreon roles in-game!";
	}
}