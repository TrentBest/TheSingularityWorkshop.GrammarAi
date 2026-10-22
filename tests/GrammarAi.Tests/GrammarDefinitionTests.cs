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
    public void Unresolved_protocol_references_are_distinct_and_keep_first_use_order()
    {
        var first = new GrammarProtocolReference(1001, 2001);
        var second = new GrammarProtocolReference(1002, 3001);
        var grammar = new GrammarBuilder(3001, "Linked", 4001)
            .Rule(5001, 4001,
                GrammarSymbol.Terminal(first),
                GrammarSymbol.Terminal(second),
                GrammarSymbol.Terminal(first))
            .Build();

        var unresolved = grammar.FindUnresolvedProtocolReferences(reference => reference == second);

        Assert.Equal(new[] { first }, unresolved);
    }

    [Fact]
    public void All_resolved_protocol_references_produce_no_unresolved_items()
    {
        var grammar = new GrammarBuilder(3001, "Linked", 4001)
            .Rule(5001, 4001,
                GrammarSymbol.Terminal(new GrammarProtocolReference(1001, 2001)))
            .Build();

        Assert.Empty(grammar.FindUnresolvedProtocolReferences(_ => true));
    }

    [Fact]
    public void Protocol_link_check_requires_a_resolver()
    {
        var grammar = new GrammarBuilder(3001, "Linked", 4001)
            .Rule(5001, 4001,
                GrammarSymbol.Terminal(new GrammarProtocolReference(1001, 2001)))
            .Build();

        Assert.Throws<ArgumentNullException>(() => grammar.FindUnresolvedProtocolReferences(null!));
    }
}
