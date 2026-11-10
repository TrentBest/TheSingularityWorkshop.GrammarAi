using TheSingularityWorkshop.GrammarAi;
using Xunit;

namespace GrammarAi.Tests;

public sealed class CSharpGrammarProfileTests
{
    [Fact]
    public void Workshop_baseline_is_pinned_to_csharp_11()
    {
        var profile = CSharpGrammarProfile.WorkshopBaseline;

        Assert.Equal(new CSharpLanguageVersion(11), profile.LanguageVersion);
        Assert.Equal("C# 11", profile.LanguageVersion.ToString());
        Assert.Equal(1, CSharpGrammarProfile.DescriptorVersion);
        Assert.True(profile.Supports(CSharpGrammarFeature.InitOnlyAccessors));
        Assert.True(profile.Supports(CSharpGrammarFeature.CheckedUserDefinedOperators));
    }

    [Fact]
    public void Feature_availability_tracks_its_introducing_language_version()
    {
        var csharp8 = new CSharpGrammarProfile(new CSharpLanguageVersion(8));
        var csharp9 = new CSharpGrammarProfile(new CSharpLanguageVersion(9));
        var csharp10 = new CSharpGrammarProfile(new CSharpLanguageVersion(10));
        var csharp11 = CSharpGrammarProfile.WorkshopBaseline;

        Assert.False(csharp8.Supports(CSharpGrammarFeature.InitOnlyAccessors));
        Assert.True(csharp9.Supports(CSharpGrammarFeature.InitOnlyAccessors));
        Assert.True(csharp10.Supports(CSharpGrammarFeature.InitOnlyAccessors));
        Assert.False(csharp10.Supports(CSharpGrammarFeature.CheckedUserDefinedOperators));
        Assert.True(csharp11.Supports(CSharpGrammarFeature.CheckedUserDefinedOperators));
    }

    [Fact]
    public void Require_supported_fails_closed_for_a_newer_feature()
    {
        var profile = new CSharpGrammarProfile(new CSharpLanguageVersion(10));

        Assert.Throws<NotSupportedException>(() =>
            profile.RequireSupported(CSharpGrammarFeature.CheckedUserDefinedOperators));
        profile.RequireSupported(CSharpGrammarFeature.InitOnlyAccessors);
    }

    [Fact]
    public void Language_version_rejects_invalid_components()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CSharpLanguageVersion(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new CSharpLanguageVersion(11, -1));
    }

    [Fact]
    public void Unknown_features_are_rejected()
    {
        var profile = CSharpGrammarProfile.WorkshopBaseline;
        Assert.Throws<ArgumentOutOfRangeException>(() => profile.Supports((CSharpGrammarFeature)999));
    }
}
