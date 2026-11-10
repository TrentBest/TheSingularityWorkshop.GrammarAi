using TheSingularityWorkshop.GrammarAi;
using Xunit;

namespace GrammarAi.Tests;

public sealed class CSharpMetadataGrammarTests
{
    private static IReadOnlyDictionary<CSharpMetadataToken, ulong> Tokens() =>
        Enum.GetValues<CSharpMetadataToken>().ToDictionary(token => token, token => (ulong)token + 1200);

    [Fact]
    public void Grammar_models_attribute_sections_and_argument_boundaries()
    {
        var tokens = Tokens();
        var grammar = CSharpMetadataGrammar.Create(3006, 1001, tokens);

        Assert.Equal(37, grammar.Rules.Count);
        Assert.Empty(grammar.FindUnreachableNonTerminals());
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 19 &&
            rule.RightHandSide.Count == 1 && rule.RightHandSide[0].Id == 1);
        Assert.Contains(grammar.Rules, rule =>  rule.LeftHandSide == 1 &&
            rule.RightHandSide.Any(symbol => symbol.Id == 2) &&
            rule.RightHandSide.Any(symbol => symbol.Id == 3));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 3 &&
            rule.RightHandSide.Any(symbol => symbol.Id == 4) &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpMetadataToken.Comma]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 5 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpMetadataToken.Identifier]) &&
            rule.RightHandSide.Any(symbol => symbol.Id == 6));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 7 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpMetadataToken.OpenParenthesis]) &&
            rule.RightHandSide.Any(symbol => symbol.Id == 8));
        Assert.All(grammar.Rules.SelectMany(rule => rule.RightHandSide)
            .Where(symbol => symbol.ProtocolReference.HasValue),
            symbol => Assert.Equal((ulong)1001, symbol.ProtocolReference!.Value.ProtocolId));
    }

    [Fact]
    public void Grammar_models_generic_parameter_lists_and_constraint_families()
    {
        var tokens = Tokens();
        var grammar = CSharpMetadataGrammar.Create(3006, 1001, tokens);

        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 19 &&
            rule.RightHandSide.Count == 1 && rule.RightHandSide[0].Id == 9);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 9 &&
            rule.RightHandSide.First().ProtocolReference?.SymbolId == tokens[CSharpMetadataToken.OpenAngle] &&
            rule.RightHandSide.Last().ProtocolReference?.SymbolId == tokens[CSharpMetadataToken.CloseAngle]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 12 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpMetadataToken.Colon]) &&
            rule.RightHandSide.Any(symbol => symbol.Id == 13));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 19 &&
            rule.RightHandSide.Count == 1 && rule.RightHandSide[0].Id == 12);
        foreach (var token in new[]
        {
            CSharpMetadataToken.ClassConstraint, CSharpMetadataToken.StructConstraint,
            CSharpMetadataToken.UnmanagedConstraint, CSharpMetadataToken.NotNullConstraint,
            CSharpMetadataToken.DefaultConstraint, CSharpMetadataToken.NewKeyword
        })
            Assert.Contains(grammar.Rules, rule => rule.RightHandSide.Any(
                symbol => symbol.ProtocolReference?.SymbolId == tokens[token]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 16 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpMetadataToken.TypeReference]));
    }

    [Fact]
    public void Missing_or_zero_token_identity_is_rejected()
    {
        var tokens = Tokens().ToDictionary(pair => pair.Key, pair => pair.Value);
        tokens.Remove(CSharpMetadataToken.TypeReference);
        Assert.Throws<ArgumentException>(() => CSharpMetadataGrammar.Create(1, 1, tokens));

        tokens = Tokens().ToDictionary(pair => pair.Key, pair => pair.Value);
        tokens[CSharpMetadataToken.Identifier] = 0;
        Assert.Throws<ArgumentException>(() => CSharpMetadataGrammar.Create(1, 1, tokens));
    }
}
