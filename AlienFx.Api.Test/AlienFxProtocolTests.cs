using AlienFx.Api.Internal;
using Xunit;

namespace AlienFx.Api.Test;

public sealed class AlienFxProtocolTests
{
	[Fact]
	public void BuildSetColorPacket_EncodesZoneColorAndLocation()
	{
		var color = AlienFxColor.FromRgb(10, 20, 30);
		var packet = AlienFxProtocol.BuildSetColorPacket(AlienFxZone.LeftZone, color, 12);

		Assert.Equal((byte)0x02, packet[0]);
		Assert.Equal((byte)0x03, packet[1]);
		Assert.Equal((byte)AlienFxZone.LeftZone, packet[2]);
		Assert.Equal((byte)10, packet[6]);
		Assert.Equal((byte)20, packet[7]);
		Assert.Equal((byte)30, packet[8]);
	}

	[Fact]
	public void BuildApplyPacket_UsesApplyCommand()
	{
		var packet = AlienFxProtocol.BuildApplyPacket(12);
		Assert.Equal((byte)0x05, packet[1]);
	}
}
