using TheSingularityWorkshop.GrammarAi;
using Xunit;

namespace GrammarAi.Tests;

public sealed class UriReferenceGrammarTests
{
    private static IReadOnlyDictionary<UriReferenceGrammarToken, ulong> Tokens() =>
        Enum.GetValues<UriReferenceGrammarToken>().ToDictionary(token => token, token => (ulong)token + 700);

    [Fact]
    public void Grammar_covers_absolute_and_relative_references_and_path_forms()
    {
        var grammar = UriReferenceGrammar.Create(5001, 1101, Tokens());

        Assert.Equal("URI-Reference-RFC3986-Bounded", grammar.Name);
        Assert.Equal(1UL, grammar.StartSymbol);
        Assert.Equal(26, grammar.Rules.Count);
        Assert.Empty(grammar.FindUnreachableNonTerminals());

        Assert.All(grammar.Rules.SelectMany(rule => rule.RightHandSide)
            .Where(symbol => symbol.ProtocolReference.HasValue),
            symbol => Assert.Equal(1101UL, symbol.ProtocolReference!.Value.ProtocolId));

        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 1 &&
            rule.RightHandSide.Count == 1 && rule.RightHandSide[0].Id == 2);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 1 &&
            rule.RightHandSide.Count == 1 && rule.RightHandSide[0].Id == 3);

        // Authority paths begin with the double-slash delimiter and may have an empty path.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 6 &&
            rule.RightHandSide.Count == 3 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == Tokens()[UriReferenceGrammarToken.DoubleSlash] &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == Tokens()[UriReferenceGrammarToken.Authority]);

        // Query and fragment are independent optional suffixes.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 13 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 13 && rule.RightHandSide.Count == 2 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == Tokens()[UriReferenceGrammarToken.QuestionMark]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 14 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 14 && rule.RightHandSide.Count == 2 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == Tokens()[UriReferenceGrammarToken.Hash]);

        // A no-scheme relative path uses its distinct segment-nz-nc lexical class.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 10 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == Tokens()[UriReferenceGrammarToken.SegmentNzNoColon]);
    }

    [Fact]
    public void Missing_or_zero_token_identity_is_rejected()
    {
        var tokens = Tokens().ToDictionary(pair => pair.Key, pair => pair.Value);
        tokens.Remove(UriReferenceGrammarToken.Authority);
        Assert.Throws<ArgumentException>(() => UriReferenceGrammar.Create(1, 1, tokens));

        tokens = Tokens().ToDictionary(pair => pair.Key, pair => pair.Value);
        tokens[UriReferenceGrammarToken.Query] = 0;
        Assert.Throws<ArgumentException>(() => UriReferenceGrammar.Create(1, 1, tokens));
    }

    [Fact]
    public void Grammar_and_category_identity_must_be_nonzero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => UriReferenceGrammar.Create(0, 1, Tokens()));
        Assert.Throws<ArgumentOutOfRangeException>(() => UriReferenceGrammar.Create(1, 0, Tokens()));
    }
}
