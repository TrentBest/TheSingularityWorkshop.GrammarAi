<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# GrammarSymbol — UML and use cases

## Class relationships

```mermaid
classDiagram
    direction TB
    class GrammarRule {
      <<consumer>>
      +RightHandSide : IReadOnlyList~GrammarSymbol~
    }
    class GrammarSymbol {
      +Id : ulong
      +ProtocolReference : GrammarProtocolReference?
      +IsNonTerminal : bool
      +IsProtocolSymbol : bool
      +NonTerminal(id) GrammarSymbol
      +Terminal(reference) GrammarSymbol
    }
    class GrammarProtocolReference {
      +ProtocolId : ulong
      +SymbolId : ulong
    }
    GrammarRule *-- GrammarSymbol : ordered RHS
    GrammarSymbol --> GrammarProtocolReference : optional terminal identity
```

## Use cases

### Create a nonterminal
Use `NonTerminal(id)` for grammar-owned symbols. The whole definition later checks that any such symbol referenced in a right-hand side has a production.

### Create a protocol terminal
Use `Terminal(reference)` to preserve the identity pair of a symbol owned by an external protocol. This does not resolve the identity against a protocol registry.

### Keep semantics separate from structure
The symbol discriminates grammar structure from external vocabulary identity; it does not parse, establish truth, or authorize actions.

See [`GrammarSymbol.md`](GrammarSymbol.md) for the class card and detailed contract.