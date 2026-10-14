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

	[Fact]
	public void FromRgb_ValidValues_ProducesColor()
	{
		var color = AlienFxColor.FromRgb(1, 2, 3);
		Assert.Equal((byte)1, color.Red);
		Assert.Equal((byte)2, color.Green);
		Assert.Equal((byte)3, color.Blue);
	}

	[Theory]
	[InlineData(-1, 0, 0)]
	[InlineData(0, -1, 0)]
	[InlineData(0, 0, -1)]
	[InlineData(256, 0, 0)]
	[InlineData(0, 256, 0)]
	[InlineData(0, 0, 256)]
	public void FromRgb_OutOfRange_ThrowsArgumentOutOfRange(int red, int green, int blue)
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => AlienFxColor.FromRgb(red, green, blue));
	}

	[Fact]
	public void FromHex_Null_ThrowsArgumentException()
	{
		Assert.Throws<ArgumentException>(() => AlienFxColor.FromHex(null!));
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void FromHex_Blank_ThrowsArgumentException(string hex)
	{
		Assert.Throws<ArgumentException>(() => AlienFxColor.FromHex(hex));
	}

	[Theory]
	[InlineData("0A")]
	[InlineData("0A64FF0A")]
	public void FromHex_WrongLength_ThrowsArgumentException(string hex)
	{
		Assert.Throws<ArgumentException>(() => AlienFxColor.FromHex(hex));
	}

	[Fact]
	public void FromHex_WithoutHash_ParsesValues()
	{
		var color = AlienFxColor.FromHex("FF00AA");
		Assert.Equal((byte)0xFF, color.Red);
		Assert.Equal((byte)0x00, color.Green);
		Assert.Equal((byte)0xAA, color.Blue);
	}
}
