namespace AlienFx.Api;

/// <summary>
/// Represents an RGB color for an AlienFX lighting zone.
/// </summary>
public readonly record struct AlienFxColor(byte Red, byte Green, byte Blue)
{
	public static AlienFxColor FromRgb(int red, int green, int blue)
	{
		if (red is < 0 or > 255)
		{
			throw new ArgumentOutOfRangeException(nameof(red));
		}

		if (green is < 0 or > 255)
		{
			throw new ArgumentOutOfRangeException(nameof(green));
		}

		if (blue is < 0 or > 255)
		{
			throw new ArgumentOutOfRangeException(nameof(blue));
		}

		return new AlienFxColor((byte)red, (byte)green, (byte)blue);
	}

	public static AlienFxColor FromHex(string hex)
	{
		if (string.IsNullOrWhiteSpace(hex))
		{
			throw new ArgumentException("Hex value must be provided.", nameof(hex));
		}

		var normalized = hex.Trim().TrimStart('#');
		if (normalized.Length != 6)
		{
			throw new ArgumentException("Hex value must be in RRGGBB format.", nameof(hex));
		}

		var red = Convert.ToByte(normalized[..2], 16);
		var green = Convert.ToByte(normalized.Substring(2, 2), 16);
		var blue = Convert.ToByte(normalized.Substring(4, 2), 16);

		return new AlienFxColor(red, green, blue);
	}

	public string ToHex() => $"#{Red:X2}{Green:X2}{Blue:X2}";
}
