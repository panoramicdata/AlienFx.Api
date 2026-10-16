using AlienFx.Api.Internal;
using Xunit;

namespace AlienFx.Api.Test;

public sealed class AlienFxClientTests
{
	[Fact]
	public async Task GetProfileAsync_ReturnsCurrentProfile()
	{
		var transport = new FakeTransport();
		var client = new AlienFxClient(new AlienFxClientOptions { InitialProfile = AlienFxProfiles.PanoramicTeal }, transport);

		var profile = await client.GetProfileAsync(CancellationToken.None);
		Assert.Equal("PanoramicTeal", profile.Name);
	}

	[Fact]
	public async Task SetProfileAsync_SendsProfileToTransport()
	{
		var transport = new FakeTransport();
		var client = new AlienFxClient(new AlienFxClientOptions(), transport);
		var profile = AlienFxProfiles.AllWhite;

		await client.SetProfileAsync(profile, CancellationToken.None);

		Assert.True(transport.EnsureConnectedCalls > 0);
		Assert.True(transport.SetColorCalls >= profile.ZoneColors.Count);
		Assert.True(transport.ApplyCalls > 0);
	}

	[Fact]
	public async Task SetProfileAsync_ByName_UsesBuiltIn()
	{
		var transport = new FakeTransport();
		var client = new AlienFxClient(new AlienFxClientOptions(), transport);

		await client.SetProfileAsync("AllWhite", CancellationToken.None);
		Assert.True(transport.ApplyCalls > 0);
	}

	[Fact]
	public void Constructor_NullOptions_ThrowsArgumentNull()
	{
		Assert.Throws<ArgumentNullException>(() => new AlienFxClient(null!, new FakeTransport()));
	}

	[Fact]
	public void Constructor_NullTransport_ThrowsArgumentNull()
	{
		Assert.Throws<ArgumentNullException>(() => new AlienFxClient(new AlienFxClientOptions(), null!));
	}

	[Fact]
	public async Task SetProfileAsync_ByName_UnknownName_ThrowsKeyNotFound()
	{
		var transport = new FakeTransport();
		var client = new AlienFxClient(new AlienFxClientOptions(), transport);

		await Assert.ThrowsAsync<KeyNotFoundException>(() => client.SetProfileAsync("DoesNotExist", CancellationToken.None));
	}

	[Fact]
	public async Task SetProfileAsync_ByName_BlankName_ThrowsArgument()
	{
		var transport = new FakeTransport();
		var client = new AlienFxClient(new AlienFxClientOptions(), transport);

		await Assert.ThrowsAsync<ArgumentException>(() => client.SetProfileAsync("   ", CancellationToken.None));
	}

	[Fact]
	public async Task SetProfileAsync_NullProfile_ThrowsArgumentNull()
	{
		var transport = new FakeTransport();
		var client = new AlienFxClient(new AlienFxClientOptions(), transport);

		await Assert.ThrowsAsync<ArgumentNullException>(() => client.SetProfileAsync((AlienFxProfile)null!, CancellationToken.None));
	}

	[Fact]
	public async Task DisposeAsync_ReleasesTransport()
	{
		var transport = new FakeTransport();
		var client = new AlienFxClient(new AlienFxClientOptions(), transport);

		await client.DisposeAsync();
		Assert.True(transport.Disposed);
	}

	private sealed class FakeTransport : IAlienFxTransport
	{
		public int EnsureConnectedCalls { get; private set; }
		public int SetColorCalls { get; private set; }
		public int ApplyCalls { get; private set; }
		public bool Disposed { get; private set; }

		public Task EnsureConnectedAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			EnsureConnectedCalls++;
			return Task.CompletedTask;
		}

		public Task WaitUntilReadyAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.CompletedTask;
		}

		public Task SetColorAsync(AlienFxZone zone, AlienFxColor _color, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			SetColorCalls++;
			return Task.CompletedTask;
		}

		public Task ApplyAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ApplyCalls++;
			return Task.CompletedTask;
		}

		public ValueTask DisposeAsync()
		{
			Disposed = true;
			return ValueTask.CompletedTask;
		}
	}
}
