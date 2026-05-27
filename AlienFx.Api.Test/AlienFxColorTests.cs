using Xunit;

namespace AlienFx.Api.Test;

public sealed class AlienFxColorTests
{
	[Fact]
	public void FromHex_ParsesRgbHex()
	{
		var color = AlienFxColor.FromHex("#0A64FF");

		Assert.Equal((byte)0x0A, color.Red);
		Assert.Equal((byte)0x64, color.Green);
		Assert.Equal((byte)0xFF, color.Blue);
	}

	[Fact]
	public void ToHex_RoundTrips()
	{
		var color = AlienFxColor.FromRgb(1, 2, 3);
		Assert.Equal("#010203", color.ToHex());
	}
}
