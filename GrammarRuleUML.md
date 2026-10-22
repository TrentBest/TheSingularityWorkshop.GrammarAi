<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# GrammarRule — UML and use cases

## Class relationships

```mermaid
classDiagram
    direction TB
    class GrammarDefinition {
      <<consumer of rules>>
      +Rules : IReadOnlyList~GrammarRule~
      +Describe() string
    }
    class GrammarRule {
      +Id : ulong
      +LeftHandSide : ulong
      +RightHandSide : IReadOnlyList~GrammarSymbol~
    }
    class GrammarSymbol {
      +Id : ulong
      +ProtocolReference : GrammarProtocolReference?
      +IsNonTerminal : bool
      +IsProtocolSymbol : bool
    }
    GrammarDefinition *-- GrammarRule : preserves declaration order
    GrammarRule *-- GrammarSymbol : preserves sequence order
```

## Use cases

### Alternative productions
Two rules may expand the same left-hand-side nonterminal. They are alternatives at the grammar level; this class does not choose which one a parser or downstream consumer uses.

### Ordered composition
The right-hand-side list preserves the declared symbol order. Consumers should not treat it as an unordered set.

### Epsilon
An empty right-hand side explicitly represents epsilon. A nonterminal with no production is a different condition and is rejected by whole-definition validation when referenced.

### Validation boundary
The rule enforces local construction invariants only. `GrammarDefinition` checks cross-rule relationships, while external protocol linking and candidate conformance remain separate responsibilities.

See [`GrammarRule.md`](GrammarRule.md) for the class card and detailed contract.