using AlienFx.Api.Internal;

namespace AlienFx.Api;

/// <summary>
/// Main API client for reading and applying AlienFX profiles.
/// </summary>
public sealed class AlienFxClient : IAsyncDisposable
{
	private readonly AlienFxClientOptions _options;
	private readonly IAlienFxTransport _transport;
	private readonly SemaphoreSlim _gate = new(1, 1);
	private AlienFxProfile _currentProfile;

	public AlienFxClient()
		: this(new AlienFxClientOptions())
	{
	}

	public AlienFxClient(AlienFxClientOptions options)
		: this(options, new AlienFxUsbTransport(options))
	{
	}

	internal AlienFxClient(AlienFxClientOptions options, IAlienFxTransport transport)
	{
		_options = options ?? throw new ArgumentNullException(nameof(options));
		_transport = transport ?? throw new ArgumentNullException(nameof(transport));
		_currentProfile = options.InitialProfile;
	}

	public async Task<AlienFxProfile> GetProfileAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
		try
		{
			return new AlienFxProfile(_currentProfile.Name, _currentProfile.ZoneColors);
		}
		finally
		{
			_gate.Release();
		}
	}

	public async Task SetProfileAsync(AlienFxProfile profile, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(profile);
		cancellationToken.ThrowIfCancellationRequested();

		if (!OperatingSystem.IsWindows())
		{
			throw new PlatformNotSupportedException("AlienFx.Api currently supports Windows only.");
		}

		await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
		try
		{
			await _transport.EnsureConnectedAsync(cancellationToken).ConfigureAwait(false);
			await _transport.WaitUntilReadyAsync(cancellationToken).ConfigureAwait(false);

			foreach (var zoneColor in profile.ZoneColors)
			{
				await _transport.SetColorAsync(zoneColor.Key, zoneColor.Value, cancellationToken).ConfigureAwait(false);
			}

			await _transport.ApplyAsync(cancellationToken).ConfigureAwait(false);
			_currentProfile = profile;
		}
		finally
		{
			_gate.Release();
		}
	}

	public async Task SetProfileAsync(string builtInProfileName, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(builtInProfileName))
		{
			throw new ArgumentException("Built-in profile name must be provided.", nameof(builtInProfileName));
		}

		var profile = AlienFxProfiles.BuiltIns.FirstOrDefault(p =>
			string.Equals(p.Name, builtInProfileName, StringComparison.OrdinalIgnoreCase));

		if (profile is null)
		{
			throw new KeyNotFoundException($"Built-in profile '{builtInProfileName}' was not found.");
		}

		await SetProfileAsync(profile, cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask DisposeAsync()
	{
		await _transport.DisposeAsync().ConfigureAwait(false);
		_gate.Dispose();
	}
}
