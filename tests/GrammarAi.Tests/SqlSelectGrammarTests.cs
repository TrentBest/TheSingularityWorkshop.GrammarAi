using TheSingularityWorkshop.GrammarAi;
using Xunit;

namespace GrammarAi.Tests;

public sealed class SqlSelectGrammarTests
{
    private static IReadOnlyDictionary<SqlSelectGrammarToken, ulong> Tokens() =>
        Enum.GetValues<SqlSelectGrammarToken>().ToDictionary(token => token, token => (ulong)token + 1300);

    [Fact]
    public void Grammar_covers_projection_sources_joins_and_boolean_predicates()
    {
        var grammar = SqlSelectGrammar.Create(7001, 1401, Tokens());

        Assert.Equal("SQL-SELECT-Core-Bounded", grammar.Name);
        Assert.Equal(1UL, grammar.StartSymbol);
        Assert.Equal(45, grammar.Rules.Count);
        Assert.Empty(grammar.FindUnreachableNonTerminals());

        Assert.All(grammar.Rules.SelectMany(rule => rule.RightHandSide)
            .Where(symbol => symbol.ProtocolReference.HasValue),
            symbol => Assert.Equal(1401UL, symbol.ProtocolReference!.Value.ProtocolId));

        // Projection lists are ordered, comma-separated, and support wildcard or expression items.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 4 &&
            rule.RightHandSide.Count == 3 &&
            rule.RightHandSide[0].IsNonTerminal && rule.RightHandSide[0].Id == 4 &&
            rule.RightHandSide[1].ProtocolReference?.SymbolId == Tokens()[SqlSelectGrammarToken.Comma] &&
            rule.RightHandSide[2].IsNonTerminal && rule.RightHandSide[2].Id == 5);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 5 &&
            rule.RightHandSide.Count == 1 && rule.RightHandSide[0].ProtocolReference?.SymbolId == Tokens()[SqlSelectGrammarToken.Star]);

        // Join and WHERE clauses are optional/repeated structural components.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 8 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 8 && rule.RightHandSide.Count == 2);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 11 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 11 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == Tokens()[SqlSelectGrammarToken.Where]);

        // OR is lower precedence than AND; NOT binds at the next tighter level. Grouping, comparisons, and NULL predicates remain explicit.
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 15 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == Tokens()[SqlSelectGrammarToken.And]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 12 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == Tokens()[SqlSelectGrammarToken.Or]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 14 &&
            rule.RightHandSide.Count == 1 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == Tokens()[SqlSelectGrammarToken.Equals]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 16 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == Tokens()[SqlSelectGrammarToken.Not]));
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 18 && rule.RightHandSide.Count == 0);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 18 &&
            rule.RightHandSide.Count == 1 &&
            rule.RightHandSide[0].ProtocolReference?.SymbolId == Tokens()[SqlSelectGrammarToken.Outer]);
        Assert.Contains(grammar.Rules, rule => rule.LeftHandSide == 13 &&
            rule.RightHandSide.Any(symbol => symbol.ProtocolReference?.SymbolId == Tokens()[SqlSelectGrammarToken.Is]));
    }

    [Fact]
    public void Missing_or_zero_token_identity_is_rejected()
    {
        var tokens = Tokens().ToDictionary(pair => pair.Key, pair => pair.Value);
        tokens.Remove(SqlSelectGrammarToken.Select);
        Assert.Throws<ArgumentException>(() => SqlSelectGrammar.Create(1, 1, tokens));

        tokens = Tokens().ToDictionary(pair => pair.Key, pair => pair.Value);
        tokens[SqlSelectGrammarToken.Identifier] = 0;
        Assert.Throws<ArgumentException>(() => SqlSelectGrammar.Create(1, 1, tokens));
    }

    [Fact]
    public void Grammar_and_category_identity_must_be_nonzero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlSelectGrammar.Create(0, 1, Tokens()));
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlSelectGrammar.Create(1, 0, Tokens()));
    }
}
