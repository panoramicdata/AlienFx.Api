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

	private sealed class FakeTransport : IAlienFxTransport
	{
		public int EnsureConnectedCalls { get; private set; }
		public int SetColorCalls { get; private set; }
		public int ApplyCalls { get; private set; }

		public Task EnsureConnectedAsync(CancellationToken cancellationToken)
		{
			EnsureConnectedCalls++;
			return Task.CompletedTask;
		}

		public Task WaitUntilReadyAsync(CancellationToken cancellationToken) => Task.CompletedTask;

		public Task SetColorAsync(AlienFxZone zone, AlienFxColor color, CancellationToken cancellationToken)
		{
			SetColorCalls++;
			return Task.CompletedTask;
		}

		public Task ApplyAsync(CancellationToken cancellationToken)
		{
			ApplyCalls++;
			return Task.CompletedTask;
		}

		public ValueTask DisposeAsync() => ValueTask.CompletedTask;
	}
}
