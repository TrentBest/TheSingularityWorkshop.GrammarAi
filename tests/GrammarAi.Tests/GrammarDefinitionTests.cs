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
    public void Description_is_deterministic()
    {
        var grammar = new GrammarBuilder(3001, "Greeting", 4001)
            .Rule(5001, 4001, GrammarSymbol.Terminal(new GrammarProtocolReference(1001, 2001)))
            .Build();

        Assert.Equal("[3001] Greeting start=[4001]\n  rule [5001] [4001] -> [1001:2001]", grammar.Describe());
    }
}