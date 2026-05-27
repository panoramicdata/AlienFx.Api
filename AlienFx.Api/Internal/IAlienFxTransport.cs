namespace AlienFx.Api.Internal;

internal interface IAlienFxTransport : IAsyncDisposable
{
	Task EnsureConnectedAsync(CancellationToken cancellationToken);

	Task WaitUntilReadyAsync(CancellationToken cancellationToken);

	Task SetColorAsync(AlienFxZone zone, AlienFxColor color, CancellationToken cancellationToken);

	Task ApplyAsync(CancellationToken cancellationToken);
}
