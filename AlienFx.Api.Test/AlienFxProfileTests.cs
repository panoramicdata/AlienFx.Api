using Xunit;

namespace AlienFx.Api.Test;

public sealed class AlienFxProfileTests
{
	[Fact]
	public void Constructor_ValidInputs_ReadsBackProperties()
	{
		var map = new Dictionary<AlienFxZone, AlienFxColor>
		{
			[AlienFxZone.LeftZone] = AlienFxColor.FromRgb(1, 2, 3),
		};
		var profile = new AlienFxProfile("Test", map);

		Assert.Equal("Test", profile.Name);
		Assert.True(profile.TryGetColor(AlienFxZone.LeftZone, out var color));
		Assert.Equal(AlienFxColor.FromRgb(1, 2, 3), color);
		Assert.False(profile.TryGetColor(AlienFxZone.RightZone, out _));
	}

	[Fact]
	public void Constructor_NullName_ThrowsArgumentException()
	{
		var map = new Dictionary<AlienFxZone, AlienFxColor>
		{
			[AlienFxZone.LeftZone] = AlienFxColor.FromRgb(1, 2, 3),
		};

		Assert.Throws<ArgumentException>(() => new AlienFxProfile(null!, map));
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void Constructor_InvalidName_ThrowsArgumentException(string name)
	{
		var map = new Dictionary<AlienFxZone, AlienFxColor>
		{
			[AlienFxZone.LeftZone] = AlienFxColor.FromRgb(1, 2, 3),
		};

		Assert.Throws<ArgumentException>(() => new AlienFxProfile(name, map));
	}

	[Fact]
	public void Constructor_NullZoneColors_ThrowsArgumentNullException()
	{
		Assert.Throws<ArgumentNullException>(() => new AlienFxProfile("Test", null!));
	}

	[Fact]
	public void Constructor_EmptyZoneColors_ThrowsArgumentException()
	{
		Assert.Throws<ArgumentException>(() => new AlienFxProfile("Test", new Dictionary<AlienFxZone, AlienFxColor>()));
	}
}
