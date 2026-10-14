using HidSharp;

namespace AlienFx.Api.Internal;

internal sealed class AlienFxUsbTransport : IAlienFxTransport
{
	private readonly AlienFxClientOptions _options;
	private readonly List<HidDevice> _devices = [];
	private readonly List<HidStream> _streams = [];

	public AlienFxUsbTransport(AlienFxClientOptions options)
	{
		_options = options;
	}

	public async Task EnsureConnectedAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		if (HasWritableStreams())
		{
			return;
		}

		await Task.Run(() => Reconnect(), cancellationToken).ConfigureAwait(false);
	}

	private bool HasWritableStreams()
	{
		return _streams.Count > 0 && _streams.All(s => s.CanWrite);
	}

	private void Reconnect()
	{
		DisposeOpenStreams();
		_devices.Clear();
		OpenDevices(GetVendorIdsToTry());
		ThrowIfNoWritableDevice(GetVendorIdsToTry());
	}

	private void OpenDevices(IReadOnlyList<int> vendorIdsToTry)
	{
		var candidates = FindCandidates(vendorIdsToTry);
		OpenStreams(PrioritizeDevices(candidates));
	}

	private void ThrowIfNoWritableDevice(IReadOnlyList<int> vendorIdsToTry)
	{
		if (_streams.Count > 0)
		{
			return;
		}

		if (!_options.ThrowIfDeviceNotFound)
		{
			return;
		}

		throw new InvalidOperationException(BuildNoDeviceMessage(vendorIdsToTry));
	}

	private string BuildNoDeviceMessage(IReadOnlyList<int> vendorIdsToTry)
	{
		var attempted = string.Join(", ", vendorIdsToTry.Select(v => $"0x{v:X4}"));
		return $"No writable AlienFX HID device found. Attempted vendor IDs: {attempted}. If your keyboard is AWCC Advanced KB, try VendorId 0x0D62 and ProductId 0xD2B0.";
	}

	private List<HidDevice> FindCandidates(IReadOnlyList<int> vendorIdsToTry)
	{
		var list = DeviceList.Local;
		var candidates = new List<HidDevice>();

		foreach (var vendorId in vendorIdsToTry)
		{
			var perVendor = _options.ProductId.HasValue
				? list.GetHidDevices(vendorId, _options.ProductId.Value)
				: list.GetHidDevices(vendorId);

			candidates.AddRange(perVendor);
		}

		return candidates;
	}

	private List<HidDevice> PrioritizeDevices(List<HidDevice> candidates)
	{
		return candidates
			.OrderByDescending(d => d.GetMaxOutputReportLength())
			.Where(d => d.GetMaxOutputReportLength() >= _options.ReportLength)
			.GroupBy(d => d.DevicePath)
			.Select(g => g.First())
			.ToList();
	}

	private void OpenStreams(List<HidDevice> devices)
	{
		foreach (var device in devices)
		{
			if (!device.TryOpen(out var stream) || stream is null)
			{
				continue;
			}

			if (!stream.CanWrite)
			{
				stream.Dispose();
				continue;
			}

			_devices.Add(device);
			_streams.Add(stream);
		}
	}

	private IReadOnlyList<int> GetVendorIdsToTry()
	{
		const int DellVid = 0x187C;
		const int DarfonVid = 0x0D62;

		if (_options.VendorId == DellVid)
		{
			return [DellVid, DarfonVid];
		}

		if (_options.VendorId == DarfonVid)
		{
			return [DarfonVid, DellVid];
		}

		return [_options.VendorId];
	}

	public Task WaitUntilReadyAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		return Task.Delay(_options.ReadyDelayMilliseconds, cancellationToken);
	}

	public Task SetColorAsync(AlienFxZone zone, AlienFxColor color, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		EnsureStreams();

		var packet = AlienFxProtocol.BuildSetColorPacket(zone, color, _options.ReportLength);
		var loopPacket = AlienFxProtocol.BuildLoopPacket(_options.ReportLength);
		var packetVariants = BuildPacketVariants(packet);
		var loopVariants = BuildPacketVariants(loopPacket);

		return Task.Run(() =>
		{
			foreach (var stream in _streams)
			{
				foreach (var packetVariant in packetVariants)
				{
					stream.Write(packetVariant);
				}

				foreach (var loopVariant in loopVariants)
				{
					stream.Write(loopVariant);
				}
			}
		}, cancellationToken);
	}

	public Task ApplyAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		EnsureStreams();

		var packet = AlienFxProtocol.BuildApplyPacket(_options.ReportLength);
		var packetVariants = BuildPacketVariants(packet);
		return Task.Run(() =>
		{
			foreach (var stream in _streams)
			{
				foreach (var packetVariant in packetVariants)
				{
					stream.Write(packetVariant);
				}
			}
		}, cancellationToken);
	}

	public ValueTask DisposeAsync()
	{
		DisposeOpenStreams();
		_devices.Clear();
		return ValueTask.CompletedTask;
	}

	private void EnsureStreams()
	{
		if (_streams.Count == 0)
		{
			throw new InvalidOperationException("AlienFX device is not connected. Call SetProfileAsync after creating a valid client.");
		}
	}

	private void DisposeOpenStreams()
	{
		foreach (var stream in _streams)
		{
			stream.Dispose();
		}

		_streams.Clear();
	}

	private static IReadOnlyList<byte[]> BuildPacketVariants(byte[] packet)
	{
		if (packet.Length == 0)
		{
			return [packet];
		}

		if (packet[0] != 0x02)
		{
			return [packet];
		}

		var zeroReportIdPacket = (byte[])packet.Clone();
		zeroReportIdPacket[0] = 0x00;
		return [packet, zeroReportIdPacket];
	}
}
