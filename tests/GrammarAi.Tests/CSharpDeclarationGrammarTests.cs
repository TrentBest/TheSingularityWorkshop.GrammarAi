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

        Assert.Equal(96, grammar.Rules.Count);
        Assert.Empty(grammar.FindUnreachableNonTerminals());
        Assert.All(grammar.Rules.SelectMany(rule => rule.RightHandSide)
            .Where(symbol => symbol.ProtocolReference.HasValue),
            symbol => Assert.Equal((ulong)1001, symbol.ProtocolReference!.Value.ProtocolId));

        // Generic methods compose metadata roots through opaque references.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 21 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 21 && rule.RightHandSide.Count == 2 &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.AttributeSectionReference]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 22 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 22 && rule.RightHandSide.Count == 2 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.GenericParameterListReference]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 23 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 23 && rule.RightHandSide.Count == 2 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ConstraintClauseReference]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 2 && rule.RightHandSide.Any(symbol => symbol.Id == 21) &&
            rule.RightHandSide.Any(symbol => symbol.Id == 22));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 2 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.BlockReference]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 2 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.Arrow]) &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ExpressionReference]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 7 &&
            rule.RightHandSide.Count == 3 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.Comma] &&
            rule.RightHandSide[1].Id == 8 && rule.RightHandSide[2].Id == 7);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 24 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 24 && rule.RightHandSide.Count == 2 &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ParameterAttributeSectionReference]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 8 && rule.RightHandSide.Count == 3 &&
            rule.RightHandSide[0].Id == 24 && rule.RightHandSide[1].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.TypeReference]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 8 &&
            rule.RightHandSide.Count == 5 &&
            rule.RightHandSide[3].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.Assignment]);
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
        // Properties, constructors, and indexers expose declaration-level attribute slots.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 3 &&
            rule.RightHandSide.Count == 7 && rule.RightHandSide[0].Id == 21);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 11 &&
            rule.RightHandSide[0].Id == 21 && rule.RightHandSide.Any(symbol =>
                symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.Identifier]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 14 &&
            rule.RightHandSide[0].Id == 21 && rule.RightHandSide.Any(symbol =>
                symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ThisKeyword]));
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
        // Indexers use the this keyword, bracketed index parameters, and the existing accessor model.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 1 &&
            rule.RightHandSide.Count == 1 && rule.RightHandSide[0].Id == 14);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 14 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ThisKeyword]) &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.OpenBracket]) &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.CloseBracket]) &&
            rule.RightHandSide.Any(symbol => symbol.Id == 6) && rule.RightHandSide.Any(symbol => symbol.Id == 9));
        // Field-like events and custom add/remove accessors are distinct from property accessors.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 1 &&
            rule.RightHandSide.Count == 1 && rule.RightHandSide[0].Id == 15);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 15 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.EventKeyword]) &&
            rule.RightHandSide.Last().ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.Semicolon]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 15 &&
            rule.RightHandSide.Any(symbol => symbol.Id == 16) &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.OpenBrace]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 16 && rule.RightHandSide.Count == 4 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.AddAccessor] &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.BlockReference] &&
            rule.RightHandSide[2].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.RemoveAccessor] &&
            rule.RightHandSide[3].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.BlockReference]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 16 && rule.RightHandSide.Count == 4 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.RemoveAccessor] &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.BlockReference] &&
            rule.RightHandSide[2].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.AddAccessor] &&
            rule.RightHandSide[3].ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.BlockReference]);
        // Ordinary operators preserve the operator token and general parameter list; conversion
        // operators distinguish implicit/explicit and require one conversion parameter.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 1 && rule.RightHandSide.Count == 1 && rule.RightHandSide[0].Id == 19);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 1 && rule.RightHandSide.Count == 1 && rule.RightHandSide[0].Id == 20);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 19 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.OperatorKeyword]) &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.OperatorToken]) &&
            rule.RightHandSide.Any(symbol => symbol.Id == 8));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 20 &&
            rule.RightHandSide.Count(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.Comma]) == 1 &&
            rule.RightHandSide.Count(symbol => symbol.Id == 8) == 2);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 19 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.Arrow]) &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ExpressionReference]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 18 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ImplicitKeyword]) &&
            rule.RightHandSide.Any(symbol => symbol.Id == 8));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 18 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ExplicitKeyword]) &&
            rule.RightHandSide.Any(symbol => symbol.Id == 8));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 18 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.Arrow]));
        // Every existing declaration family now has an attribute-prefixed structural alternative.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 4 &&
            rule.RightHandSide[0].Id == 21 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.Semicolon]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 15 &&
            rule.RightHandSide[0].Id == 21 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.EventKeyword]) &&
            rule.RightHandSide.Last().ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.Semicolon]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 15 &&
            rule.RightHandSide[0].Id == 21 && rule.RightHandSide.Any(symbol => symbol.Id == 16));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 19 &&
            rule.RightHandSide[0].Id == 21 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.OperatorToken]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 19 &&
            rule.RightHandSide[0].Id == 21 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.CheckedKeyword]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 20 &&
            rule.RightHandSide[0].Id == 21 && rule.RightHandSide.Count(symbol => symbol.Id == 8) == 2);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 20 &&
            rule.RightHandSide[0].Id == 21 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.CheckedOperatorToken]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 18 &&
            rule.RightHandSide[0].Id == 21 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ImplicitKeyword]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 18 &&
            rule.RightHandSide[0].Id == 21 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ExplicitKeyword]));
        // Checked unary and binary forms are syntactically distinct and preserve exact operand counts.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 19 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.CheckedKeyword]) &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.CheckedOperatorToken]) &&
            rule.RightHandSide.Count(symbol => symbol.Id == 8) == 1);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 20 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.CheckedKeyword]) &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.CheckedOperatorToken]) &&
            rule.RightHandSide.Count(symbol => symbol.Id == 8) == 2 &&
            rule.RightHandSide.Count(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.Comma]) == 1);
    }

    [Fact]
    public void Explicit_interface_member_shapes_use_host_owned_specifiers()
    {
        var tokens = Tokens();
        var grammar = CSharpDeclarationGrammar.Create(3004, 1001, tokens);

        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 2 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.TypeReference]) &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ExplicitInterfaceSpecifierReference]) &&
            rule.RightHandSide.Any(symbol => symbol.Id == 22));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 3 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.TypeReference]) &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ExplicitInterfaceSpecifierReference]) &&
            rule.RightHandSide.Any(symbol => symbol.Id == 9));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 14 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.TypeReference]) &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ExplicitInterfaceSpecifierReference]) &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ThisKeyword]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 15 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.TypeReference]) &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpDeclarationToken.ExplicitInterfaceSpecifierReference]) &&
            rule.RightHandSide.Any(symbol => symbol.Id == 16));
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
