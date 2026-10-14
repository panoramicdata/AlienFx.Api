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

	[Fact]
	public void BuildLoopPacket_UsesLoopCommand()
	{
		var packet = AlienFxProtocol.BuildLoopPacket(12);
		Assert.Equal((byte)0x02, packet[0]);
		Assert.Equal((byte)0x04, packet[1]);
	}

	[Fact]
	public void BuildSetColorPacket_UnknownZone_ThrowsNotSupportedException()
	{
		Assert.Throws<NotSupportedException>(() => AlienFxProtocol.BuildSetColorPacket((AlienFxZone)99, AlienFxColor.FromRgb(1, 2, 3), 12));
	}

	[Fact]
	public void BuildSetColorPacket_MacroKeys_UsesCommand0x83()
	{
		var packet = AlienFxProtocol.BuildSetColorPacket(AlienFxZone.MacroKeys, AlienFxColor.FromRgb(1, 2, 3), 12);
		Assert.Equal((byte)0x83, packet[1]);
	}

	[Fact]
	public void BuildSetColorPacket_PowerButton_UsesCommand0x01AndLocationByte()
	{
		var packet = AlienFxProtocol.BuildSetColorPacket(AlienFxZone.PowerButton, AlienFxColor.FromRgb(1, 2, 3), 12);
		Assert.Equal((byte)0x01, packet[1]);
		Assert.Equal((byte)0x01, packet[4]);
	}

	[Fact]
	public void EmptyPacket_RejectsShortLength()
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => AlienFxProtocol.EmptyPacket(8));
	}

	[Fact]
	public void EmptyPacket_ReturnsRequestedLength()
	{
		Assert.Equal(9, AlienFxProtocol.EmptyPacket(9).Length);
	}
}
