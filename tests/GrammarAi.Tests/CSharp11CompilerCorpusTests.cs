using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace GrammarAi.Tests;

/// <summary>Compiler-backed syntax/version fixtures for the declared C# 11 baseline. These validate dialect expectations, not GrammarAi parsing.</summary>
public sealed class CSharp11CompilerCorpusTests
{
    private static readonly MetadataReference[] PlatformReferences =
        ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? string.Empty)
        .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
        .Select(path => MetadataReference.CreateFromFile(path))
        .ToArray();

    public static TheoryData<string, string> CSharp11PositiveCases => new()
    {
        { "init-only accessor", "public sealed class Sample { public int Value { get; init; } }" },
        { "checked binary operator pair", "public sealed class Number { public static Number operator +(Number left, Number right) => left; public static Number operator checked +(Number left, Number right) => left; }" },
        { "checked unary operator pair", "public sealed class Number { public static Number operator ++(Number value) => value; public static Number operator checked ++(Number value) => value; }" },
        { "method expression body and optional parameter", "public sealed class Sample { public int Add(int value, int amount = 1) => value + amount; }" },
        { "indexer with init accessor", "public sealed class Sample { public int this[int index] { get => index; init { } } }" },
        { "custom event accessors", "public sealed class Sample { public event System.Action Changed { add { } remove { } } }" },
        { "explicit-interface member implementations", "using System; public interface IContract { void Run(); int Value { get; } int this[int index] { get; } event Action Changed; } public sealed class Sample : IContract { void IContract.Run() { } int IContract.Value => 1; int IContract.this[int index] => index; event Action IContract.Changed { add { } remove { } } }" },
        { "finalizer block and expression body", "public sealed class Sample { ~Sample() { } } public sealed class Other { ~Other() => System.GC.KeepAlive(this); }" },
        { "constructor initializer", "public class Base { public Base(int value) { } } public sealed class Sample : Base { public Sample() : base(1) { } }" },
        { "attributes and generic class constraints", "[System.Obsolete(\"sample\")] public sealed class Sample<T> where T : class, new() { }" },
        { "generic method unmanaged constraint", "public sealed class Sample { public void Use<TValue>() where TValue : unmanaged { } }" },
        { "generic method notnull constraint", "public sealed class Sample { public void Use<TValue>() where TValue : notnull { } }" },
        { "parameter and return-target attributes", "using System; [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)] public sealed class MarkerAttribute : Attribute { } public sealed class Sample { [return: Marker] public int Read([Marker] int value) => value; }" },
        { "parameter attributes with generic method metadata", "using System; [AttributeUsage(AttributeTargets.Parameter)] public sealed class MarkerAttribute : Attribute { } public sealed class Sample { [System.Obsolete] public TResult Convert<TSource, TResult>([Marker] TSource value) where TSource : notnull where TResult : class, new() => new TResult(); }" }
    };

    public static TheoryData<string, string, string> VersionBoundaryCases => new()
    {
        { "init-only accessor requires C# 9", "8.0", "public sealed class Sample { public int Value { get; init; } }" },
        { "checked operators require C# 11", "10.0", "public sealed class Number { public static Number operator +(Number left, Number right) => left; public static Number operator checked +(Number left, Number right) => left; }" }
    };

    [Theory]
    [MemberData(nameof(CSharp11PositiveCases))]
    public void Csharp11_positive_corpus_compiles(string name, string source)
    {
        var errors = Compile(source, LanguageVersion.CSharp11).GetDiagnostics()
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).ToArray();
        Assert.True(errors.Length == 0, $"{name} should compile under C# 11:{Environment.NewLine}{string.Join(Environment.NewLine, errors.Select(d => d.ToString()))}");
    }

    [Theory]
    [MemberData(nameof(VersionBoundaryCases))]
    public void Earlier_language_versions_reject_later_features(string name, string version, string source)
    {
        Assert.True(LanguageVersionFacts.TryParse(version, out var parsed), $"Invalid test language version '{version}'.");
        var errors = Compile(source, parsed).GetDiagnostics()
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).ToArray();
        Assert.NotEmpty(errors);
        Assert.Contains(errors, diagnostic => diagnostic.Id is "CS8773" or "CS8936" or "CS8400");
    }

    [Theory]
    [InlineData("public sealed class Sample { public int this[int index { get; } }")]
    [InlineData("public sealed class Number { public static Number operator +(Number left, Number middle, Number right) => left; }")]
    [InlineData("public sealed class Sample { public event System.Action Changed { add { } } }")]
    [InlineData("[System.Obsolete(\"sample\") public sealed class Sample { }")]
    [InlineData("public sealed class Sample { public Sample(int value) : this(value) { } }")]
    [InlineData("public sealed class Sample { public void Use<in TValue>() { } }")]
    public void Negative_corpus_rejects_malformed_or_incomplete_declarations(string source)
    {
        var errors = Compile(source, LanguageVersion.CSharp11).GetDiagnostics()
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).ToArray();
        Assert.NotEmpty(errors);
    }

    private static CSharpCompilation Compile(string source, LanguageVersion version)
    {
        var tree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(languageVersion: version, documentationMode: DocumentationMode.None));
        return CSharpCompilation.Create("CSharpGrammarCorpus", new[] { tree }, PlatformReferences,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}