namespace TheSingularityWorkshop.GrammarAi;

/// <summary>Language features represented by the bounded C# grammar resources.</summary>
public enum CSharpGrammarFeature
{
    /// <summary>Init-only property and indexer accessors, introduced in C# 9.</summary>
    InitOnlyAccessors,

    /// <summary>Checked user-defined unary and binary operators, introduced in C# 11.</summary>
    CheckedUserDefinedOperators
}

/// <summary>A numeric C# language version, independent of a GrammarAi grammar/schema version.</summary>
public readonly record struct CSharpLanguageVersion
{
    public int Major { get; }
    public int Minor { get; }

    public CSharpLanguageVersion(int major, int minor = 0)
    {
        if (major < 1) throw new ArgumentOutOfRangeException(nameof(major));
        if (minor < 0) throw new ArgumentOutOfRangeException(nameof(minor));
        Major = major;
        Minor = minor;
    }

    /// <summary>Returns whether this language version is at least the supplied version.</summary>
    public bool IsAtLeast(int major, int minor = 0) =>
        Major > major || (Major == major && Minor >= minor);

    public override string ToString() => Minor == 0
        ? $"C# {Major}"
        : $"C# {Major}.{Minor}";
}

/// <summary>Declares the language-version baseline used to describe a bounded C# grammar resource.</summary>
/// <remarks>
/// This descriptor is deliberately separate from GrammarDefinition identity/version. It reports whether
/// selected features belong to the declared language version; it does not parse source, filter productions,
/// or certify that a partial grammar conforms to the complete C# specification.
/// </remarks>
public sealed class CSharpGrammarProfile
{
    /// <summary>Version of this profile descriptor contract.</summary>
    public const int DescriptorVersion = 1;

    /// <summary>The Workshop's current bounded C# grammar baseline.</summary>
    public static CSharpGrammarProfile WorkshopBaseline { get; } =
        new(new CSharpLanguageVersion(11));

    public CSharpLanguageVersion LanguageVersion { get; }

    public CSharpGrammarProfile(CSharpLanguageVersion languageVersion)
    {
        LanguageVersion = languageVersion;
    }

    /// <summary>Reports whether a known grammar feature is available in the declared language version.</summary>
    public bool Supports(CSharpGrammarFeature feature) => feature switch
    {
        CSharpGrammarFeature.InitOnlyAccessors => LanguageVersion.IsAtLeast(9),
        CSharpGrammarFeature.CheckedUserDefinedOperators => LanguageVersion.IsAtLeast(11),
        _ => throw new ArgumentOutOfRangeException(nameof(feature), feature, "Unknown C# grammar feature.")
    };

    /// <summary>Throws when a selected grammar feature is newer than the declared language version.</summary>
    public void RequireSupported(CSharpGrammarFeature feature)
    {
        if (!Supports(feature))
            throw new NotSupportedException(
                $"{LanguageVersion} does not support the C# grammar feature '{feature}'.");
    }
}
