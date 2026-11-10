using TheSingularityWorkshop.GrammarAi;
using Xunit;

namespace GrammarAi.Tests;

public sealed class CSharpTypeGrammarTests
{
    private static IReadOnlyDictionary<CSharpTypeToken, ulong> Tokens() =>
        Enum.GetValues<CSharpTypeToken>().ToDictionary(token => token, token => (ulong)token + 700);

    [Fact]
    public void Grammar_covers_named_qualified_generic_nullable_array_and_tuple_types()
    {
        var grammar = CSharpTypeGrammar.Create(3003, 1001, Tokens());

        Assert.Equal(21, grammar.Rules.Count);
        Assert.Empty(grammar.FindUnreachableNonTerminals());
        Assert.All(grammar.Rules.SelectMany(rule => rule.RightHandSide)
            .Where(symbol => symbol.ProtocolReference.HasValue),
            symbol => Assert.Equal((ulong)1001, symbol.ProtocolReference!.Value.ProtocolId));

        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 12 &&
            rule.RightHandSide.Count == 3 &&
            rule.RightHandSide[0].Id == 12 &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == Tokens()[CSharpTypeToken.Dot] &&
            rule.RightHandSide[2].ProtocolReference?.SymbolId == Tokens()[CSharpTypeToken.Identifier]);

        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 4 &&
            rule.RightHandSide.Count == 4 &&
            rule.RightHandSide[0].Id == 5 &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == Tokens()[CSharpTypeToken.LessThan] &&
            rule.RightHandSide[2].Id == 6 &&
            rule.RightHandSide[3].ProtocolReference?.SymbolId == Tokens()[CSharpTypeToken.GreaterThan]);

        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 7 &&
            rule.RightHandSide.Count == 3 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == Tokens()[CSharpTypeToken.OpenParenthesis] &&
            rule.RightHandSide[1].Id == 8 &&
            rule.RightHandSide[2].ProtocolReference?.SymbolId == Tokens()[CSharpTypeToken.CloseParenthesis]);
    }

    [Fact]
    public void Array_rank_supports_zero_or_more_commas_between_brackets()
    {
        var grammar = CSharpTypeGrammar.Create(3003, 1001, Tokens());

        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 9 &&
            rule.RightHandSide.Count == 3 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == Tokens()[CSharpTypeToken.OpenBracket] &&
            rule.RightHandSide[1].Id == 10 &&
            rule.RightHandSide[2].ProtocolReference?.SymbolId == Tokens()[CSharpTypeToken.CloseBracket]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 10 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 10 && rule.RightHandSide.Count == 1 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == Tokens()[CSharpTypeToken.Comma]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 10 && rule.RightHandSide.Count == 2 &&
            rule.RightHandSide[0].Id == 10 &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == Tokens()[CSharpTypeToken.Comma]);
    }

    [Fact]
    public void Missing_or_zero_token_identity_is_rejected()
    {
        var tokens = Tokens().ToDictionary(pair => pair.Key, pair => pair.Value);
        tokens.Remove(CSharpTypeToken.LessThan);
        Assert.Throws<ArgumentException>(() => CSharpTypeGrammar.Create(1, 1, tokens));

        tokens = Tokens().ToDictionary(pair => pair.Key, pair => pair.Value);
        tokens[CSharpTypeToken.Comma] = 0;
        Assert.Throws<ArgumentException>(() => CSharpTypeGrammar.Create(1, 1, tokens));
    }

    [Fact]
    public void Grammar_and_category_identity_must_be_nonzero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CSharpTypeGrammar.Create(0, 1, Tokens()));
        Assert.Throws<ArgumentOutOfRangeException>(() => CSharpTypeGrammar.Create(1, 0, Tokens()));
    }
}
