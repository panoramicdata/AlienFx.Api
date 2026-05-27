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
	public void BuiltIns_DoesNotContainLegacyAllAlwaysAll()
	{
		Assert.DoesNotContain(AlienFxProfiles.BuiltIns, p => p.Name.Contains("AlwaysAll", StringComparison.OrdinalIgnoreCase));
		Assert.Contains(AlienFxProfiles.BuiltIns, p => p.Name == "AllWhite");
	}
}
