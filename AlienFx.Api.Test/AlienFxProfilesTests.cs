using Xunit;

namespace AlienFx.Api.Test;

public sealed class AlienFxProfilesTests
{
	[Fact]
	public void LegacyAllAlwaysAll_ContainsAllKnownZones()
	{
		var profile = AlienFxProfiles.LegacyAllAlwaysAll;
		var expected = AlienFx.Api.Internal.AlienFxProtocol.GetKnownLightingZones();

		Assert.Equal(expected.Count, profile.ZoneColors.Count);
		foreach (var zone in expected)
		{
			Assert.True(profile.ZoneColors.ContainsKey(zone));
		}
	}

	[Fact]
	public void AllZonesSolid_SingleArgument_UsesDefaultName()
	{
		var profile = AlienFxProfiles.AllZonesSolid(AlienFxColor.FromHex("#123456"));
		Assert.Equal("AllZonesSolid", profile.Name);
	}

	[Fact]
	public void PanoramicTeal_HasTealColor()
	{
		var profile = AlienFxProfiles.PanoramicTeal;
		Assert.True(profile.TryGetColor(AlienFxZone.LeftZone, out var color));
		Assert.Equal((byte)0x00, color.Red);
		Assert.Equal((byte)0xA8, color.Green);
		Assert.Equal((byte)0xA8, color.Blue);
	}

	[Fact]
	public void BuiltIns_DoesNotContainLegacyAllAlwaysAll()
	{
		Assert.DoesNotContain(AlienFxProfiles.BuiltIns, p => p.Name.Contains("AlwaysAll", StringComparison.OrdinalIgnoreCase));
		Assert.Contains(AlienFxProfiles.BuiltIns, p => p.Name == "AllWhite");
	}
}
