<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# GrammarSymbol — terminal or nonterminal

## Purpose

`GrammarSymbol` is the value used in a production's right-hand-side sequence. It represents one of two distinct things:

- **Nonterminal:** an integer identity owned by the grammar; it can be expanded by grammar productions.
- **Protocol terminal:** a symbol identity owned by an external protocol, represented by a `GrammarProtocolReference`.

The distinction is explicit in the value: `ProtocolReference` is null for a nonterminal and populated for a protocol terminal. `IsNonTerminal` and `IsProtocolSymbol` expose those complementary cases.

## Responsibility boundary

- **Owns:** a nonzero symbol ID and an optional protocol reference.
- **Factory methods:** `NonTerminal(id)` creates a grammar-owned nonterminal; `Terminal(reference)` creates a protocol-terminal symbol.
- **Does not do:** look up protocol definitions, parse candidate text, determine semantic truth, or authorize an operation.
- **Whole-grammar checks:** `GrammarDefinition` ensures nonterminals referenced on right-hand sides have productions. External terminal resolution is separate.

## UML class card

```mermaid
classDiagram
    direction TB
    class GrammarRule {
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
    GrammarSymbol --> GrammarProtocolReference : optional terminal reference
```

## Use cases

1. **Declare an expandable category:** use `GrammarSymbol.NonTerminal(id)` for a symbol that has grammar productions.
2. **Reference a protocol-owned terminal:** use `GrammarSymbol.Terminal(reference)` with the external protocol/symbol identity.
3. **Validate the complete grammar:** let the built definition check that referenced nonterminals have productions.
4. **Resolve and interpret:** let the host resolve terminal references and interpret application meaning; the symbol object itself is not a semantic validator.

## Source of truth

Implementation: [`GrammarSymbol.cs`](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi/blob/development/GrammarSymbol.cs).

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
