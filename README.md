# PatreonPlugin
This is a plugin for managing Patreon perks in the game with large customizability

## Config Additions
### General
Config Option | Value Type | Default Value | Description
--- | :---: | :---: | ---
patreon_items | Dictionary | **Empty** | The default items to give to Patreon supporters (Format: Key = Class ID, Value = Item ID)
patreon_tag | String | **Empty** | The default tag to use for Patreon supporters
patreon_tag_colour | String | default | The default tag colour to use for Patreon supporters
patreon_tag_auto_refresh | Boolean | False | If true, the tags will be updated whenever a Patreon supporter's class is set
patreon_auto_reserve | Boolean | False | If true, Patreon supporters will automatically get Reserved Slot entries created
\<Rank Name\>_items | Dictionary | **Empty** | The default items to give to Patreon supporters with the specified rank (Format: Key = Class ID, Value = Item ID)
\<Rank Name\>_tag | String | **Empty** | The default tag to use for Patreon supporters with the specified rank
\<Rank Name\>_tag_colour | String | default | The default tag colour to use for Patreon supporters with the specified rank
\<Rank Name\>_tag_auto_refresh | Boolean | False | If true, the tags will be updated whenever a Patreon supporter with the specified rank's class is set
\<Rank Name\>_auto_reserve | Boolean | False | If true, Patreon supporters with the specified rank will automatically get Reserved Slot entries created

### Patreon List Config
This config is stored as `PatreonList.txt` in the server root directory.
The custom configuration file for this is very open in formatting.

For the formatting of the files, there are some loose rules:
- You must have a SteamID in the line for a Patreon entry, the first 17 digit number in the line will be used for this
- Any line without a SteamID will be ignored

For custom values per Patron, the format for tags should be `--tag value_without_spaces` or `--tag "value with spaces"`

The available tags for setting custom values of the Patron's tag and items:
- For items: `-i`, `--items`, `--customitems`
- For tags: `-t`, `--tag`, `--customtag`, `--custom`
- For tag colours: `-c`, `--colour`, `--color`
- For auto refreshing tags: `-a`, `--auto`, `--autorefresh`
- For automated Reserved Slots tags: `-s`, `--slot`, `--reserved`, `--reservedslot`
- For setting a player's rank: `-r`, `--rank`