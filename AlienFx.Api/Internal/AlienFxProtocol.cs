namespace AlienFx.Api.Internal;

internal static class AlienFxProtocol
{
	private const byte ReportId = 0x02;

	private static readonly IReadOnlyDictionary<AlienFxZone, int> ZoneLocations = new Dictionary<AlienFxZone, int>
	{
		[AlienFxZone.LeftZone] = 0x000008,
		[AlienFxZone.LeftMiddleZone] = 0x000004,
		[AlienFxZone.RightZone] = 0x000001,
		[AlienFxZone.RightMiddleZone] = 0x000002,
		[AlienFxZone.MacroKeys] = 0x000000,
		[AlienFxZone.AlienFrontLogo] = 0x000040,
		[AlienFxZone.LeftPanelTop] = 0x001000,
		[AlienFxZone.LeftPanelBottom] = 0x000400,
		[AlienFxZone.RightPanelTop] = 0x002000,
		[AlienFxZone.RightPanelBottom] = 0x000800,
		[AlienFxZone.PowerButton] = 0x00000D,
		[AlienFxZone.AlienBackLogo] = 0x000020,
		[AlienFxZone.TouchPad] = 0x000080,
	};

	public static IReadOnlyList<AlienFxZone> GetKnownLightingZones() => ZoneLocations.Keys.OrderBy(z => (int)z).ToArray();

	public static byte[] BuildSetColorPacket(AlienFxZone zone, AlienFxColor color, int reportLength)
	{
		if (!ZoneLocations.TryGetValue(zone, out var location))
		{
			throw new NotSupportedException($"Zone '{zone}' is not supported.");
		}

		var packet = EmptyPacket(reportLength);
		packet[0] = ReportId;
		packet[1] = 0x03;
		packet[2] = (byte)zone;
		packet[3] = (byte)((location & 0xFF0000) >> 16);
		packet[4] = (byte)((location & 0x00FF00) >> 8);
		packet[5] = (byte)(location & 0x0000FF);
		packet[6] = color.Red;
		packet[7] = color.Green;
		packet[8] = color.Blue;

		if (zone == AlienFxZone.MacroKeys)
		{
			packet[1] = 0x83;
		}

		if (zone == AlienFxZone.PowerButton)
		{
			packet[1] = 0x01;
			packet[4] = 0x01;
		}

		return packet;
	}

	public static byte[] BuildApplyPacket(int reportLength)
	{
		var packet = EmptyPacket(reportLength);
		packet[0] = ReportId;
		packet[1] = 0x05;
		return packet;
	}

	public static byte[] BuildLoopPacket(int reportLength)
	{
		var packet = EmptyPacket(reportLength);
		packet[0] = ReportId;
		packet[1] = 0x04;
		return packet;
	}

	public static byte[] EmptyPacket(int reportLength)
	{
		if (reportLength < 9)
		{
			throw new ArgumentOutOfRangeException(nameof(reportLength), "Report length must be at least 9 bytes.");
		}

		return new byte[reportLength];
	}
}
