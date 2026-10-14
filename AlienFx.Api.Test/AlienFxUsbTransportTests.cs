using AlienFx.Api.Internal;
using Xunit;

namespace AlienFx.Api.Test;

public sealed class AlienFxUsbTransportTests
{
	[Fact]
	public void SetColorAsync_WithCancelledToken_ThrowsOperationCanceled()
	{
		var transport = new AlienFxUsbTransport(new AlienFxClientOptions());
		using var cts = new CancellationTokenSource();
		cts.Cancel();

		Assert.ThrowsAny<OperationCanceledException>(() =>
			transport.SetColorAsync(AlienFxZone.LeftZone, AlienFxColor.FromRgb(1, 2, 3), cts.Token).GetAwaiter().GetResult());
	}

	[Fact]
	public void ApplyAsync_WithCancelledToken_ThrowsOperationCanceled()
	{
		var transport = new AlienFxUsbTransport(new AlienFxClientOptions());
		using var cts = new CancellationTokenSource();
		cts.Cancel();

		Assert.ThrowsAny<OperationCanceledException>(() =>
			transport.ApplyAsync(cts.Token).GetAwaiter().GetResult());
	}

	[Fact]
	public async Task WaitUntilReadyAsync_WithCancelledToken_ThrowsOperationCanceled()
	{
		var transport = new AlienFxUsbTransport(new AlienFxClientOptions { ReadyDelayMilliseconds = 1 });
		using var cts = new CancellationTokenSource();
		cts.Cancel();

		await Assert.ThrowsAnyAsync<OperationCanceledException>(() => transport.WaitUntilReadyAsync(cts.Token));
	}

	[Fact]
	public async Task EnsureConnectedAsync_WithCancelledToken_ThrowsOperationCanceled()
	{
		var transport = new AlienFxUsbTransport(new AlienFxClientOptions());
		using var cts = new CancellationTokenSource();
		cts.Cancel();

		await Assert.ThrowsAnyAsync<OperationCanceledException>(() => transport.EnsureConnectedAsync(cts.Token));
	}

	[Fact]
	public async Task SetColorAsync_WithoutDevice_ThrowsWhenRequired()
	{
		var transport = new AlienFxUsbTransport(new AlienFxClientOptions { ThrowIfDeviceNotFound = true });

		await Assert.ThrowsAnyAsync<Exception>(() => transport.SetColorAsync(AlienFxZone.LeftZone, AlienFxColor.FromRgb(1, 2, 3), CancellationToken.None));
	}

	[Fact]
	public async Task ApplyAsync_WithoutDevice_ThrowsWhenRequired()
	{
		var transport = new AlienFxUsbTransport(new AlienFxClientOptions { ThrowIfDeviceNotFound = true });

		await Assert.ThrowsAnyAsync<Exception>(() => transport.ApplyAsync(CancellationToken.None));
	}

	[Fact]
	public async Task EnsureConnectedAsync_NoDevice_DoesNotThrowWhenNotRequired()
	{
		var transport = new AlienFxUsbTransport(new AlienFxClientOptions { ThrowIfDeviceNotFound = false });
		await transport.EnsureConnectedAsync(CancellationToken.None);
	}

	[Fact]
	public async Task DisposeAsync_DisposesWithoutError()
	{
		var transport = new AlienFxUsbTransport(new AlienFxClientOptions());
		await transport.DisposeAsync();
	}

}
