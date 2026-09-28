using TheSingularityWorkshop.GrammarAi;

var grammar = new GrammarBuilder(3001, "Greeting", 4001)
    .Rule(5001, 4001,
        GrammarSymbol.Terminal(new GrammarProtocolReference(1001, 2001)))
    .Rule(5002, 4001,
        GrammarSymbol.Terminal(new GrammarProtocolReference(1001, 2002)))
    .Build();

Console.WriteLine("GRAMMAR");
Console.WriteLine(grammar.Describe());
Console.WriteLine();
Console.WriteLine("WHAT: [1001:2001] and [1001:2002] are owned by ProtocolAI.");
Console.WriteLine("HOW: [4001] may resolve to either terminal.");
