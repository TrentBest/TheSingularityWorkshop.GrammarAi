using TheSingularityWorkshop.GrammarAi;
using Xunit;

namespace GrammarAi.Tests;

public sealed class CSharpExpressionGrammarTests
{
    private static IReadOnlyDictionary<CSharpExpressionToken, ulong> CreateVocabulary() =>
        Enum.GetValues<CSharpExpressionToken>()
            .ToDictionary(token => token, token => (ulong)token + 100);

    [Fact]
    public void Creates_precedence_layered_recursive_expression_grammar()
    {
        var grammar = CSharpExpressionGrammar.Create(700, 900, CreateVocabulary());

        Assert.Equal(1UL, grammar.StartSymbol);
        Assert.Equal("CSharpCoreExpressions", grammar.Name);
        Assert.Equal(20, grammar.Rules.Count);
        Assert.Empty(grammar.FindUnreachableNonTerminals());
        Assert.Empty(grammar.FindUnresolvedProtocolReferences(_ => true));

        // Multiplicative binds more tightly than additive; additive binds more tightly than relational.
        Assert.Equal(new ulong[] { 8 }, grammar.Rules
            .Where(rule => rule.LeftHandSide == 7)
            .SelectMany(rule => rule.RightHandSide)
            .Where(symbol => symbol.IsNonTerminal)
            .Select(symbol => symbol.Id)
            .Distinct()
            .Where(id => id == 8));

        // Left-recursive binary layers encode left associativity; assignment is right-recursive.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 7 &&
            rule.RightHandSide.Count == 3 && rule.RightHandSide[0].IsNonTerminal &&
            rule.RightHandSide[0].Id == 7);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 2 &&
            rule.RightHandSide.Count == 3 && rule.RightHandSide[2].IsNonTerminal &&
            rule.RightHandSide[2].Id == 2);

        Assert.All(grammar.Rules.SelectMany(rule => rule.RightHandSide).Where(symbol => symbol.IsProtocolSymbol),
            symbol => Assert.Equal(900UL, symbol.ProtocolReference!.Value.ProtocolId));
    }

    [Fact]
    public void Rejects_missing_or_zero_vocabulary_member_ids()
    {
        var missing = CreateVocabulary().Where(pair => pair.Key != CSharpExpressionToken.Additive)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        Assert.Throws<ArgumentException>(() => CSharpExpressionGrammar.Create(700, 900, missing));

        var zero = CreateVocabulary().ToDictionary(pair => pair.Key, pair => pair.Value);
        zero[CSharpExpressionToken.Literal] = 0;
        Assert.Throws<ArgumentException>(() => CSharpExpressionGrammar.Create(700, 900, zero));
    }

    [Fact]
    public void Rejects_zero_grammar_or_protocol_identity()
    {
        var vocabulary = CreateVocabulary();
        Assert.Throws<ArgumentOutOfRangeException>(() => CSharpExpressionGrammar.Create(0, 900, vocabulary));
        Assert.Throws<ArgumentOutOfRangeException>(() => CSharpExpressionGrammar.Create(700, 0, vocabulary));
    }
}
