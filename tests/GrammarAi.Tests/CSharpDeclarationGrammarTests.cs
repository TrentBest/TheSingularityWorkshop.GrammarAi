using TheSingularityWorkshop.GrammarAi;
using Xunit;

namespace GrammarAi.Tests;

public sealed class CSharpDeclarationGrammarTests
{
    private static IReadOnlyDictionary<CSharpDeclarationToken, ulong> Tokens() =>
        Enum.GetValues<CSharpDeclarationToken>().ToDictionary(token => token, token => (ulong)token + 900);

    [Fact]
    public void Grammar_covers_member_kinds_and_parameter_shapes()
    {
        var tokens = Tokens();
        var grammar = CSharpDeclarationGrammar.Create(3004, 1001, tokens);

        Assert.Equal(37, grammar.Rules.Count);
        Assert.Empty(grammar.FindUnreachableNonTerminals());
        Assert.All(grammar.Rules.SelectMany(rule => rule.RightHandSide)
            .Where(symbol => symbol.ProtocolReference.HasValue),
            symbol => Assert.Equal((ulong)1001, symbol.ProtocolReference!.Value.ProtocolId));

        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 2 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.BlockReference]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 2 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.Arrow]) &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ExpressionReference]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 7 &&
            rule.RightHandSide.Count == 3 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.Comma] &&
            rule.RightHandSide[1].Id == 8 && rule.RightHandSide[2].Id == 7);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 8 &&
            rule.RightHandSide.Count == 4 &&
            rule.RightHandSide[2].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.Assignment]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 10 &&
            rule.RightHandSide.Count == 2 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.InitAccessor]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 10 &&
            rule.RightHandSide.Count == 2 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.GetAccessor] &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.BlockReference]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 10 &&
            rule.RightHandSide.Count == 4 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.SetAccessor] &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.Arrow] &&
            rule.RightHandSide[2].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ExpressionReference] &&
            rule.RightHandSide[3].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.Semicolon]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 10 &&
            rule.RightHandSide.Count == 2 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.InitAccessor] &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.BlockReference]);
        // Constructors have no return-type slot; parameters and invocation arguments are distinct shapes.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 11 &&
            rule.RightHandSide.Any(symbol => symbol.Id == 12) &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.Identifier]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 12 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 12 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ThisKeyword]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 12 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.BaseKeyword]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 13 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 13 &&
            rule.RightHandSide.Count == 1 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ExpressionReference]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 13 &&
            rule.RightHandSide.Count == 3 &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.Comma]);
    }

    [Fact]
    public void Empty_and_repeated_lists_are_representable()
    {
        var grammar = CSharpDeclarationGrammar.Create(3004, 1001, Tokens());

        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 5 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 6 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 9 && rule.RightHandSide.Count == 1);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 7 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 9 && rule.RightHandSide.Count == 2);
    }

    [Fact]
    public void Missing_or_zero_token_identity_is_rejected()
    {
        var tokens = Tokens().ToDictionary(pair => pair.Key, pair => pair.Value);
        tokens.Remove(CSharpDeclarationToken.TypeReference);
        Assert.Throws<ArgumentException>(() => CSharpDeclarationGrammar.Create(1, 1, tokens));

        tokens = Tokens().ToDictionary(pair => pair.Key, pair => pair.Value);
        tokens[CSharpDeclarationToken.Modifier] = 0;
        Assert.Throws<ArgumentException>(() => CSharpDeclarationGrammar.Create(1, 1, tokens));
    }

    [Fact]
    public void Grammar_and_category_identity_must_be_nonzero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CSharpDeclarationGrammar.Create(0, 1, Tokens()));
        Assert.Throws<ArgumentOutOfRangeException>(() => CSharpDeclarationGrammar.Create(1, 0, Tokens()));
    }
}
