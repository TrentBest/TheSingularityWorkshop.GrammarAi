using TheSingularityWorkshop.GrammarAi;
using Xunit;

namespace GrammarAi.Tests;

public sealed class CSharpTypeDeclarationGrammarTests
{
    private static IReadOnlyDictionary<CSharpTypeDeclarationToken, ulong> Tokens() =>
        Enum.GetValues<CSharpTypeDeclarationToken>().ToDictionary(token => token, token => (ulong)token + 1500);

    [Fact]
    public void Grammar_composes_type_declaration_kinds_and_metadata_slots()
    {
        var tokens = Tokens();
        var grammar = CSharpTypeDeclarationGrammar.Create(3007, 1001, tokens);

        Assert.Equal(25, grammar.Rules.Count);
        Assert.Empty(grammar.FindUnreachableNonTerminals());
        Assert.All(grammar.Rules.SelectMany(rule => rule.RightHandSide)
            .Where(symbol => symbol.ProtocolReference.HasValue),
            symbol => Assert.Equal((ulong)1001, symbol.ProtocolReference!.Value.ProtocolId));

        Assert.Equal(new ulong[] { 2, 3, 4, 5, 6, 7 },
            grammar.Rules.Where(rule => rule.LeftHandSide == 1).Select(rule => rule.RightHandSide.Single().Id).ToArray());
        foreach (var declaration in new ulong[] { 2, 3, 4, 5, 6 })
        {
            Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == declaration &&
                rule.RightHandSide.Any(symbol => symbol.Id == 8) &&
                rule.RightHandSide.Any(symbol => symbol.Id == 9) &&
                rule.RightHandSide.Any(symbol => symbol.Id == 10) &&
                rule.RightHandSide.Any(symbol => symbol.Id == 11) &&
                rule.RightHandSide.Any(symbol => symbol.Id == 12) &&
                rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpTypeDeclarationToken.TypeBodyReference]));
        }

        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 7 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpTypeDeclarationToken.TypeReference]) &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpTypeDeclarationToken.ParameterListReference]) &&
            rule.RightHandSide.Any(symbol => symbol.Id == 10));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 8 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 8 && rule.RightHandSide.Count == 2 &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == tokens[CSharpTypeDeclarationToken.AttributeSectionReference]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 10 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 10 && rule.RightHandSide.Count == 1 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == tokens[CSharpTypeDeclarationToken.GenericParameterListReference]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 12 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 12 && rule.RightHandSide.Count == 2 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == tokens[CSharpTypeDeclarationToken.ConstraintClauseReference]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 13 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 13 && rule.RightHandSide.Count == 3 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpTypeDeclarationToken.ParameterListReference]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 6 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpTypeDeclarationToken.RecordKeyword]) &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == tokens[CSharpTypeDeclarationToken.StructKeyword]));
    }

    [Fact]
    public void Missing_or_zero_token_identity_is_rejected()
    {
        var tokens = Tokens().ToDictionary(pair => pair.Key, pair => pair.Value);
        tokens.Remove(CSharpTypeDeclarationToken.TypeBodyReference);
        Assert.Throws<ArgumentException>(() => CSharpTypeDeclarationGrammar.Create(1, 1, tokens));

        tokens = Tokens().ToDictionary(pair => pair.Key, pair => pair.Value);
        tokens[CSharpTypeDeclarationToken.Identifier] = 0;
        Assert.Throws<ArgumentException>(() => CSharpTypeDeclarationGrammar.Create(1, 1, tokens));
    }
}
