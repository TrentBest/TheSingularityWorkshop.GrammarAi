using TheSingularityWorkshop.GrammarAi;
using Xunit;

namespace GrammarAi.Tests;

public sealed class JsonGrammarTests
{
    private static IReadOnlyDictionary<JsonGrammarToken, ulong> Tokens() =>
        Enum.GetValues<JsonGrammarToken>().ToDictionary(token => token, token => (ulong)token + 500);

    [Fact]
    public void Grammar_covers_rfc8259_values_objects_arrays_and_ordered_lists()
    {
        var grammar = JsonGrammar.Create(4001, 1001, Tokens());

        Assert.Equal("JSON-RFC8259", grammar.Name);
        Assert.Equal(1UL, grammar.StartSymbol);
        Assert.Equal(16, grammar.Rules.Count);
        Assert.Empty(grammar.FindUnreachableNonTerminals());

        Assert.All(grammar.Rules.SelectMany(rule => rule.RightHandSide)
            .Where(symbol => symbol.ProtocolReference.HasValue),
            symbol => Assert.Equal(1001UL, symbol.ProtocolReference!.Value.ProtocolId));

        // A root value may be an object, array, string, number, or one of the three literals.
        Assert.Equal(7, grammar.Rules.Count(rule => rule.LeftHandSide == 1));

        // Objects and arrays each support empty and nonempty forms.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 2 && rule.RightHandSide.Count == 2);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 2 && rule.RightHandSide.Count == 3);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 5 && rule.RightHandSide.Count == 2);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 5 && rule.RightHandSide.Count == 3);

        // Recursive list productions preserve order and prohibit implicit trailing separators.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 3 &&
            rule.RightHandSide.Count == 3 &&
            rule.RightHandSide[0].IsNonTerminal && rule.RightHandSide[0].Id == 3 &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == Tokens()[JsonGrammarToken.Comma] &&
            rule.RightHandSide[2].IsNonTerminal && rule.RightHandSide[2].Id == 4);

        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 6 &&
            rule.RightHandSide.Count == 3 &&
            rule.RightHandSide[0].IsNonTerminal && rule.RightHandSide[0].Id == 6 &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == Tokens()[JsonGrammarToken.Comma] &&
            rule.RightHandSide[2].IsNonTerminal && rule.RightHandSide[2].Id == 1);
    }

    [Fact]
    public void Missing_or_zero_token_identity_is_rejected()
    {
        var tokens = Tokens().ToDictionary(pair => pair.Key, pair => pair.Value);
        tokens.Remove(JsonGrammarToken.String);
        Assert.Throws<ArgumentException>(() => JsonGrammar.Create(1, 1, tokens));

        tokens = Tokens().ToDictionary(pair => pair.Key, pair => pair.Value);
        tokens[JsonGrammarToken.Number] = 0;
        Assert.Throws<ArgumentException>(() => JsonGrammar.Create(1, 1, tokens));
    }

    [Fact]
    public void Grammar_and_category_identity_must_be_nonzero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => JsonGrammar.Create(0, 1, Tokens()));
        Assert.Throws<ArgumentOutOfRangeException>(() => JsonGrammar.Create(1, 0, Tokens()));
    }
}
