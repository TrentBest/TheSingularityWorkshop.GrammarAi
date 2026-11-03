<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# Chapter 5 — Worked example: a small command language

## Learning objectives

Follow a grammar from vocabulary ownership through structural definition, external linking, candidate checking, and host authorization.

## 1. Define the vocabulary first

Assume an application owns two protocols:

\`\`\`text
Protocol 7100 — Actions
  7101 — inspect
  7102 — prepare

Protocol 7200 — Stations
  7201 — forge
  7202 — laboratory
\`\`\`

These names are explanatory labels. GrammarAi stores the integer identities; the application and its protocols own their meanings.

## 2. Describe the intended shape

The application wants a request containing one action followed by one station:

\`\`\`text
[4001] Request -> [4002] [4003]
[4002] Action  -> [7100:7101]
[4002] Action  -> [7100:7102]
[4003] Station -> [7200:7201]
[4003] Station -> [7200:7202]
\`\`\`

The first rule is an ordered sequence. The two Action rules are alternatives, as are the two Station rules.

## 3. Build the structural artifact

\`\`\`csharp
using TheSingularityWorkshop.GrammarAi;

var grammar = new GrammarBuilder(3001, "WorkshopRequest", 4001)
    .Rule(5001, 4001,
        GrammarSymbol.NonTerminal(4002),
        GrammarSymbol.NonTerminal(4003))
    .Rule(5002, 4002,
        GrammarSymbol.Terminal(new GrammarProtocolReference(7100, 7101)))
    .Rule(5003, 4002,
        GrammarSymbol.Terminal(new GrammarProtocolReference(7100, 7102)))
    .Rule(5004, 4003,
        GrammarSymbol.Terminal(new GrammarProtocolReference(7200, 7201)))
    .Rule(5005, 4003,
        GrammarSymbol.Terminal(new GrammarProtocolReference(7200, 7202)))
    .Build();

var description = grammar.Describe();
var unreachable = grammar.FindUnreachableNonTerminals();
\`\`\`

This example uses the current public API shapes in \`GrammarBuilder.cs\`, \`GrammarSymbol.cs\`, and \`GrammarProtocolReference.cs\`. It constructs a grammar definition and obtains its deterministic description and reachability findings. It does not parse text or invoke a model.

## 4. Complete the host-owned steps

A real consumer must still:

1. Resolve each protocol/symbol pair against the intended protocol definitions and versions.
2. Obtain a candidate in a representation the consumer can inspect.
3. Use a real parser or validator to determine whether that candidate conforms to this grammar.
4. Apply user permissions, application policy, operational preconditions, and any required confirmation.
5. Execute only through the host's authorized execution path.

A structurally valid proposal such as \`prepare + forge\` is still only a proposal. GrammarAi does not grant permission or operate a station.

## 5. What the example demonstrates

The grammar makes allowed structural combinations explicit while preserving protocol ownership. This is useful when a consumer needs to inspect, document, compare, or translate a structural contract.

It does not demonstrate a complete command runtime. The linking, candidate-conformance, policy, and execution stages are intentionally outside this package's current core.

## Evidence in the repository

- Builder API: \`GrammarBuilder.cs\`.
- Terminal and nonterminal factories: \`GrammarSymbol.cs\`.
- External identity pair: \`GrammarProtocolReference.cs\`.
- Deterministic description: \`GrammarDefinition.Describe()\`.
- Existing construction tests: \`tests/GrammarAi.Tests/GrammarDefinitionTests.cs\`.
- Operational consumer guidance: [Consuming GrammarAi](../CONSUMING.md).

## Further study

Continue with [the validation and ownership chapter](04-validation-and-ownership.md), then consult the [API class guides](../../DOCUMENTATION_INDEX.md#source-level-class-guides) when implementing a consumer.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
