using Xunit;
using TheSingularityWorkshop.GrammarAi;

namespace GrammarAi.Tests;

public sealed class GrammarDefinitionTests
{
    [Fact]
    public void Builder_connects_nonterminals_to_external_protocol_symbols()
    {
        var grammar = new GrammarBuilder(3001, "Greeting", 4001)
            .Rule(5001, 4001, GrammarSymbol.Terminal(new GrammarProtocolReference(1001, 2001)))
            .Rule(5002, 4001, GrammarSymbol.Terminal(new GrammarProtocolReference(1001, 2002)))
            .Build();

        Assert.Equal((ulong)3001, grammar.Id);
        Assert.Equal((ulong)4001, grammar.StartSymbol);
        Assert.Equal(2, grammar.Rules.Count);
        Assert.Equal((ulong)1001, grammar.Rules[0].RightHandSide[0].ProtocolReference!.Value.ProtocolId);
        Assert.Equal((ulong)2001, grammar.Rules[0].RightHandSide[0].ProtocolReference!.Value.SymbolId);
    }

    [Fact]
    public void Referenced_nonterminals_must_have_productions()
    {
        Assert.Throws<ArgumentException>(() => new GrammarBuilder(3001, "Broken", 4001)
            .Rule(5001, 4001, GrammarSymbol.NonTerminal(4999))
            .Build());
    }

    [Fact]
    public void Builder_snapshot_is_immutable_after_subsequent_builder_changes()
    {
        var builder = new GrammarBuilder(3001, "Greeting", 4001)
            .Rule(5001, 4001, GrammarSymbol.Terminal(new GrammarProtocolReference(1001, 2001)));
        var first = builder.Build();

        builder.Rule(5002, 4001, GrammarSymbol.Terminal(new GrammarProtocolReference(1001, 2002)));

        Assert.Single(first.Rules);
    }

    [Fact]
    public void Duplicate_rule_ids_are_rejected()
    {
        Assert.Throws<ArgumentException>(() => new GrammarBuilder(3001, "Greeting", 4001)
            .Rule(5001, 4001, GrammarSymbol.Terminal(new GrammarProtocolReference(1001, 2001)))
            .Rule(5001, 4001, GrammarSymbol.Terminal(new GrammarProtocolReference(1001, 2002)))
            .Build());
    }

    [Fact]
    public void Recursive_nonterminals_are_allowed_as_structure()
    {
        var grammar = new GrammarBuilder(3001, "Recursive", 4001)
            .Rule(5001, 4001, GrammarSymbol.NonTerminal(4001))
            .Build();

        Assert.Equal((ulong)4001, grammar.Rules[0].RightHandSide[0].Id);
    }

    [Fact]
    public void Description_is_deterministic()
    {
        var grammar = new GrammarBuilder(3001, "Greeting", 4001)
            .Rule(5001, 4001, GrammarSymbol.Terminal(new GrammarProtocolReference(1001, 2001)))
            .Build();

        Assert.Equal("[3001] Greeting start=[4001]\n  rule [5001] [4001] -> [1001:2001]", grammar.Describe());
    }
    [Fact]
    public void Default_uninitialized_symbol_is_rejected_in_a_production()
    {
        Assert.Throws<ArgumentException>(() => new GrammarBuilder(3001, "Broken", 4001)
            .Rule(5001, 4001, default(GrammarSymbol))
            .Build());
    }

    [Fact]
    public void Empty_production_is_explicit_epsilon()
    {
        var grammar = new GrammarBuilder(3001, "Optional", 4001)
            .Rule(5001, 4001)
            .Build();

        Assert.Empty(grammar.Rules[0].RightHandSide);
        Assert.Equal("[3001] Optional start=[4001]\n  rule [5001] [4001] -> ε", grammar.Describe());
    }

    [Fact]
    public void Grammar_and_rule_identities_must_be_nonzero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GrammarBuilder(0, "Invalid", 4001));
        Assert.Throws<ArgumentOutOfRangeException>(() => new GrammarBuilder(3001, "Invalid", 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new GrammarRule(0, 4001, []));
        Assert.Throws<ArgumentOutOfRangeException>(() => new GrammarRule(5001, 0, []));
    }

    [Fact]
    public void Protocol_references_require_nonzero_protocol_and_symbol_ids()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GrammarProtocolReference(0, 2001));
        Assert.Throws<ArgumentOutOfRangeException>(() => new GrammarProtocolReference(1001, 0));
    }

    [Fact]
    public void Start_symbol_must_have_a_production()
    {
        var builder = new GrammarBuilder(3001, "MissingStart", 4001)
            .Rule(5001, 4002);

        Assert.Throws<ArgumentException>(() => builder.Build());
    }

    [Fact]
    public void Grammar_definition_and_production_inputs_are_bounded()
    {
        Assert.Throws<ArgumentException>(() => new GrammarBuilder(1, new string('g', GrammarLimits.MaxNameCharacters + 1), 1));
        Assert.Throws<ArgumentException>(() => new GrammarRule(1, 1,
            Enumerable.Repeat(GrammarSymbol.NonTerminal(1), GrammarLimits.MaxSymbolsPerRule + 1)));
        Assert.Throws<ArgumentException>(() => new GrammarBuilder(1, "bounded", 1)
            .Rule(1, 1, Enumerable.Repeat(GrammarSymbol.NonTerminal(1), GrammarLimits.MaxSymbolsPerRule + 1).ToArray()));
    }

    [Fact]
    public void Grammar_builder_stops_accepting_rules_at_the_limit()
    {
        var builder = new GrammarBuilder(1, "bounded", 1);
        for (var i = 1; i <= GrammarLimits.MaxRules; i++)
            builder.Rule((ulong)i, 1);
        Assert.Throws<InvalidOperationException>(() => builder.Rule(GrammarLimits.MaxRules + 1UL, 1));
    }
    [Fact]
    public void Grammar_builder_bounds_aggregate_production_symbols()
    {
        var builder = new GrammarBuilder(1, "bounded-total", 1);
        var production = Enumerable.Repeat(GrammarSymbol.NonTerminal(1), GrammarLimits.MaxSymbolsPerRule).ToArray();
        for (var i = 1; i <= GrammarLimits.MaxTotalSymbols / GrammarLimits.MaxSymbolsPerRule + 1; i++)
            builder.Rule((ulong)i, 1, production);
        Assert.Throws<ArgumentException>(() => builder.Build());
    }
    [Fact]
    public void Grammar_rule_rejects_uninitialized_symbols()
    {
        Assert.Throws<ArgumentException>(() => new GrammarRule(1, 1, [default]));
    }
}
