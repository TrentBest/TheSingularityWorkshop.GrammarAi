<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" />
  </a>
</p>

# GrammarAi — Documentation

GrammarAi defines how symbols can be arranged into grammar productions. The grammar owns its structure; referenced terminal identities remain owned by their protocol definitions.

## Start here

| Your goal | Read |
|---|---|
| Understand the problem and the WHAT/HOW boundary | [README](README.md) |
| Add GrammarAi to a .NET application | [Consuming GrammarAi](docs/CONSUMING.md) |
| Define and inspect a grammar | [Examples](docs/EXAMPLES.md) |
| See protocol identities become grammar structure | [Gallery](docs/GALLERY.md) |
| Understand terminal references and nonterminal rules | [Consuming guide: protocol identities](docs/CONSUMING.md#2-start-with-protocol-identities) |
| Understand the role of recursive productions and epsilon | [Examples](docs/EXAMPLES.md) |

## Source-level class guides

These companion pages describe the current public types and link to focused relationship diagrams and use cases:

| Type | Contract and purpose | UML and use cases |
|---|---|---|
| `GrammarBuilder` | [Class guide](GrammarBuilder.md) | [UML](GrammarBuilderUML.md) |
| `GrammarDefinition` | [Class guide](GrammarDefinition.md) | [UML](GrammarDefinitionUML.md) |
| `GrammarRule` | [Class guide](GrammarRule.md) | [UML](GrammarRuleUML.md) |
| `GrammarSymbol` | [Class guide](GrammarSymbol.md) | [UML](GrammarSymbolUML.md) |
| `GrammarProtocolReference` | [Class guide](GrammarProtocolReference.md) | [UML](GrammarProtocolReferenceUML.md) |

## Where it fits

- [ProtocolAi](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi) owns the vocabulary and identities referenced by grammar terminals.
- [HeadlessAi](https://github.com/TrentBest/TheSingularityWorkshop.HeadlessAi) provides direct HTTP communication with configured AI-capable endpoints.
- [FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS) composes reusable MicroBundle capabilities into a runtime assembly.

GrammarAi does not call a model, parse generated text, authorize actions, or execute commands. The host connects protocol identities to the application meaning and behavior it owns.

**Explore the Workshop:** [Find packages on NuGet](https://www.nuget.org/packages?q=TheSingularityWorkshop) · [Browse the repository guide](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/development/docs/WORKSHOP_REPOSITORIES.md).

---

<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://github.com/TrentBest.png?size=200" alt="The Singularity Workshop on GitHub" width="200">
  </a>
</p>

<p align="center"><a href="https://github.com/TrentBest">GitHub</a> · <a href="https://coderlegion.com/">Coder Legion</a> · <a href="https://www.patreon.com/c/TheSingularityWorkshop">Patreon</a></p>
