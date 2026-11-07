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
| Understand the intended integer-first knowledge mapping model | [Knowledge mapping model](docs/KNOWLEDGE_MAPPING_MODEL.md) |
| Review typed node/slot graphs and the v1/v2 proposal contract | [Typed structural schema](docs/STRUCTURAL_SCHEMA.md) |
| Find where GrammarAi can help other Workshop repositories | [Ecosystem applications](docs/ECOSYSTEM_APPLICATIONS.md) |\n| Review implemented capabilities and the remaining domain gaps | [Domain completeness audit](docs/DOMAIN_COMPLETENESS.md) |
| Follow the textbook-style theory learning path | [GrammarAi Theory](docs/THEORY.md) |
| Add GrammarAi to a .NET application | [Consuming GrammarAi](docs/CONSUMING.md) |
| Understand the host/LLM exchange design and its ownership boundaries | [AI Exchange design note](docs/AI_EXCHANGE.md) |
| Define and inspect a grammar | [Examples](docs/EXAMPLES.md) |
| See protocol identities become grammar structure | [Gallery](docs/GALLERY.md) |
| Understand terminal references and nonterminal rules | [Consuming guide: protocol identities](docs/CONSUMING.md#2-start-with-protocol-identities) |
| Understand recursion, epsilon, and reachability | [Theory chapter 3](docs/theory/03-productions-recursion.md) |
| Define bounded observer conditions and gate-transition intent | [Observer grammar](docs/OBSERVER_GRAMMAR.md) |

## Theory learning path

The explanatory material is split into five progressive chapters:

1. [From vocabulary to language](docs/theory/01-why-grammar.md)
2. [The grammar model](docs/theory/02-grammar-model.md)
3. [Productions, recursion, and reachability](docs/theory/03-productions-recursion.md)
4. [Structural validation and ownership boundaries](docs/theory/04-validation-and-ownership.md)
5. [Worked example: a small command language](docs/theory/05-worked-example.md)

The theory pages explain concepts and map claims to current source and tests. Operational setup remains in the consuming guide; public type contracts remain in the class guides below.

## Source-level class guides

These companion pages describe the current public types and link to focused relationship diagrams and use cases:

| Type | Contract and purpose | UML and use cases |
|---|---|---|
| `GrammarBuilder` | [Class guide](GrammarBuilder.md) | [UML](GrammarBuilderUML.md) |
| `GrammarDefinition` | [Class guide](GrammarDefinition.md) | [UML](GrammarDefinitionUML.md) |
| `GrammarRule` | [Class guide](GrammarRule.md) | [UML](GrammarRuleUML.md) |
| `GrammarSymbol` | [Class guide](GrammarSymbol.md) | [UML](GrammarSymbolUML.md) |
| `GrammarProtocolReference` | [Class guide](GrammarProtocolReference.md) | [UML](GrammarProtocolReferenceUML.md) |

## AI agent onboarding

Before changing this repository, read [AGENTS.md](AGENTS.md) and the [shared FSM_COS Documentation Standard](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/development/DOCUMENTATION_STANDARD.md). The agent guide records GrammarAi's structural boundary, external protocol-reference ownership, branch policy, and verification requirements.

## Where it fits

- [ProtocolAi](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi) owns the vocabulary and identities referenced by grammar terminals.
- [HeadlessAi](https://github.com/TrentBest/TheSingularityWorkshop.HeadlessAi) provides direct HTTP communication with configured AI-capable endpoints.
- [FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS) composes reusable MicroBundle capabilities into a runtime assembly.

GrammarAi does not call a model, parse generated text, authorize actions, or execute commands. The host connects protocol identities to the application meaning and behavior it owns.

**Explore the Workshop:** [Find packages on NuGet](https://www.nuget.org/packages?q=TheSingularityWorkshop) · [Browse the repository guide](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/development/docs/WORKSHOP_REPOSITORIES.md).

---

<p align="center"><a href="https://github.com/TrentBest">GitHub</a> · <a href="https://coderlegion.com/">Coder Legion</a> · <a href="https://www.patreon.com/c/TheSingularityWorkshop">Patreon</a></p>

- [Typed Structural Schema](docs/STRUCTURAL_SCHEMA.md) — grammar-owned node identities, contextual slots, versioned graph instances, and the compact integer-first proposal wire format.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
