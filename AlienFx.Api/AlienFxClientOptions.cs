namespace AlienFx.Api;

/// <summary>
/// Options used by <see cref="AlienFxClient"/>.
/// </summary>
public sealed class AlienFxClientOptions
{
	public int VendorId { get; init; } = 0x187C;

	public int? ProductId { get; init; }

	public int ReportLength { get; init; } = 12;

	public int ReadyDelayMilliseconds { get; init; } = 35;

	public bool ThrowIfDeviceNotFound { get; init; } = true;

	public AlienFxProfile InitialProfile { get; init; } = AlienFxProfiles.AllWhite;
}
