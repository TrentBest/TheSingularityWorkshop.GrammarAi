using TheSingularityWorkshop.GrammarAi;
using Xunit;

namespace GrammarAi.Tests;

public sealed class CSharpExpressionGrammarTests
{
    private static IReadOnlyDictionary<CSharpExpressionToken, ulong> Tokens() =>
        Enum.GetValues<CSharpExpressionToken>().ToDictionary(token => token, token => (ulong)token + 300);

    [Fact]
    public void Grammar_covers_postfix_member_invocation_and_element_access()
    {
        var grammar = CSharpExpressionGrammar.Create(3002, 1001, Tokens());

        Assert.Equal(27, grammar.Rules.Count);
        Assert.Empty(grammar.FindUnreachableNonTerminals());
        Assert.All(grammar.Rules.SelectMany(rule => rule.RightHandSide)
            .Where(symbol => symbol.ProtocolReference.HasValue),
            symbol => Assert.Equal((ulong)1001, symbol.ProtocolReference!.Value.ProtocolId));

        // Postfix recursion makes chains such as object.Member(args)[index] composable.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 11 &&
            rule.RightHandSide.Count == 3 &&
            rule.RightHandSide[0].IsNonTerminal && rule.RightHandSide[0].Id == 11 &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == Tokens()[CSharpExpressionToken.Dot] &&
            rule.RightHandSide[2].ProtocolReference?.SymbolId == Tokens()[CSharpExpressionToken.Identifier]);

        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 11 &&
            rule.RightHandSide.Count == 3 &&
            rule.RightHandSide[0].IsNonTerminal && rule.RightHandSide[0].Id == 11 &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == Tokens()[CSharpExpressionToken.OpenParenthesis] &&
            rule.RightHandSide[2].ProtocolReference?.SymbolId == Tokens()[CSharpExpressionToken.CloseParenthesis]);

        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 11 &&
            rule.RightHandSide.Count == 4 &&
            rule.RightHandSide[0].IsNonTerminal && rule.RightHandSide[0].Id == 11 &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == Tokens()[CSharpExpressionToken.OpenBracket] &&
            rule.RightHandSide[2].IsNonTerminal && rule.RightHandSide[2].Id == 1 &&
            rule.RightHandSide[3].ProtocolReference?.SymbolId == Tokens()[CSharpExpressionToken.CloseBracket]);
    }

    [Fact]
    public void Invocation_supports_empty_and_nonempty_argument_lists()
    {
        var grammar = CSharpExpressionGrammar.Create(3002, 1001, Tokens());

        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 11 &&
            rule.RightHandSide.Count == 3 &&
            rule.RightHandSide[0].Id == 11 &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == Tokens()[CSharpExpressionToken.OpenParenthesis] &&
            rule.RightHandSide[2].ProtocolReference?.SymbolId == Tokens()[CSharpExpressionToken.CloseParenthesis]);

        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 11 &&
            rule.RightHandSide.Count == 4 &&
            rule.RightHandSide[0].Id == 11 &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == Tokens()[CSharpExpressionToken.OpenParenthesis] &&
            rule.RightHandSide[2].Id == 12 &&
            rule.RightHandSide[3].ProtocolReference?.SymbolId == Tokens()[CSharpExpressionToken.CloseParenthesis]);

        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 12 &&
            rule.RightHandSide.Count == 1 && rule.RightHandSide[0].Id == 2);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 12 &&
            rule.RightHandSide.Count == 3 && rule.RightHandSide[0].Id == 12 &&
            rule.RightHandSide[2].Id == 2);
    }

    [Fact]
    public void Missing_or_zero_token_identity_is_rejected()
    {
        var tokens = Tokens().ToDictionary(pair => pair.Key, pair => pair.Value);
        tokens.Remove(CSharpExpressionToken.Dot);
        Assert.Throws<ArgumentException>(() => CSharpExpressionGrammar.Create(1, 1, tokens));

        tokens = Tokens().ToDictionary(pair => pair.Key, pair => pair.Value);
        tokens[CSharpExpressionToken.Comma] = 0;
        Assert.Throws<ArgumentException>(() => CSharpExpressionGrammar.Create(1, 1, tokens));
    }

    [Fact]
    public void Grammar_and_category_identity_must_be_nonzero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CSharpExpressionGrammar.Create(0, 1, Tokens()));
        Assert.Throws<ArgumentOutOfRangeException>(() => CSharpExpressionGrammar.Create(1, 0, Tokens()));
    }
}
