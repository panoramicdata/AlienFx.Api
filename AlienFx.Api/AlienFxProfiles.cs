using AlienFx.Api.Internal;

namespace AlienFx.Api;

/// <summary>
/// Built-in profiles for quick setup.
/// </summary>
public static class AlienFxProfiles
{
	private static readonly IReadOnlyList<AlienFxZone> AllZones = AlienFxProtocol.GetKnownLightingZones();

	public static AlienFxProfile AllWhite => AllZonesSolid(AlienFxColor.FromHex("#FFFFFF"), "AllWhite");

	public static AlienFxProfile PanoramicTeal => AllZonesSolid(AlienFxColor.FromHex("#00A8A8"), "PanoramicTeal");

	/// <summary>
	/// Captures the previous legacy default that was used before it was removed from built-ins.
	/// </summary>
	public static AlienFxProfile LegacyAllAlwaysAll => AllZonesSolid(AlienFxColor.FromHex("#00FFFF"), "LegacyAllAlwaysAll");

	public static IReadOnlyList<AlienFxProfile> BuiltIns { get; } =
	[
		AllWhite,
		PanoramicTeal,
	];

	public static AlienFxProfile AllZonesSolid(AlienFxColor color, string profileName = "AllZonesSolid")
	{
		var map = new Dictionary<AlienFxZone, AlienFxColor>();
		foreach (var zone in AllZones)
		{
			map[zone] = color;
		}

		return new AlienFxProfile(profileName, map);
	}
}
