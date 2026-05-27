namespace AlienFx.Api;

/// <summary>
/// Represents a named lighting profile across one or more zones.
/// </summary>
public sealed class AlienFxProfile
{
	private readonly IReadOnlyDictionary<AlienFxZone, AlienFxColor> _zoneColors;

	public AlienFxProfile(string name, IReadOnlyDictionary<AlienFxZone, AlienFxColor> zoneColors)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Profile name must be provided.", nameof(name));
		}

		if (zoneColors is null)
		{
			throw new ArgumentNullException(nameof(zoneColors));
		}

		if (zoneColors.Count == 0)
		{
			throw new ArgumentException("At least one zone color is required.", nameof(zoneColors));
		}

		Name = name;
		_zoneColors = new Dictionary<AlienFxZone, AlienFxColor>(zoneColors);
	}

	public string Name { get; }

	public IReadOnlyDictionary<AlienFxZone, AlienFxColor> ZoneColors => _zoneColors;

	public bool TryGetColor(AlienFxZone zone, out AlienFxColor color) => _zoneColors.TryGetValue(zone, out color);
}
