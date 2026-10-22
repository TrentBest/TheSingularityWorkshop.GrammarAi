<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# GrammarProtocolReference — UML and use cases

## Class relationships

```mermaid
classDiagram
    direction TB
    class GrammarConsumer {
      <<host>>
      +ResolveProtocolReference(reference)
      +ApplyPolicy()
    }
    class GrammarSymbol {
      +Id : ulong
      +ProtocolReference : GrammarProtocolReference?
      +IsProtocolSymbol : bool
    }
    class GrammarProtocolReference {
      +ProtocolId : ulong
      +SymbolId : ulong
    }
    GrammarConsumer ..> GrammarProtocolReference : resolves externally
    GrammarSymbol --> GrammarProtocolReference : stores for terminal
```

## Use cases

### Resolve external identity
A host reads `ProtocolId` and `SymbolId` and looks them up in the protocol definitions selected for the application. GrammarAi's value object does not perform that lookup.

### Preserve vocabulary boundaries
The reference lets a grammar use a protocol-owned terminal without becoming the owner of that terminal's meaning.

### Avoid overclaiming
A nonzero reference is syntactically valid as an identifier pair; it does not prove that the protocol or symbol exists, that versions are compatible, or that a use is authorized.

See [`GrammarProtocolReference.md`](GrammarProtocolReference.md) for the class card and contract.