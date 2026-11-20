<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" />
  </a>
</p>

# GrammarAi — Documentation

GrammarAi has two structural representations: an integer-backed production grammar and a typed node/slot knowledge-map model. Both reference externally owned vocabulary identities; neither owns domain meaning, authorization, or execution.

## Start here

| Your goal | Read |
|---|---|
| Understand the problem and the WHAT/HOW boundary | [README](README.md) |
| Install and consume the current package/source candidate | [Consuming GrammarAi](docs/CONSUMING.md) |
| Understand the integer-first knowledge-mapping model | [Knowledge mapping model](docs/KNOWLEDGE_MAPPING_MODEL.md) |
| Review typed node/slot graphs and the v1/v2 proposal contract | [Typed structural schema](docs/STRUCTURAL_SCHEMA.md) |
| See how GrammarAi could fit SingularityBot without misrepresenting current adoption | [GrammarAi and SingularityBot](docs/INTEGRATION_SINGULARITYBOT.md) |
| Find where GrammarAi can help other Workshop repositories | [Ecosystem applications](docs/ECOSYSTEM_APPLICATIONS.md) |
| Review repository-by-repository adoption decisions and coordination status | [Ecosystem adoption audit](docs/ECOSYSTEM_ADOPTION_AUDIT.md) |
| Read tailored integration proposals for specific consumers | [Consumer adoption guides](docs/ecosystem-adoption/README.md) |
| Review implemented capabilities and remaining domain gaps | [Domain completeness audit](docs/DOMAIN_COMPLETENESS.md) |
| Understand the RFC 5234 / RFC 7405 ABNF model and its validation boundary | [ABNF representation](docs/ABNF_REPRESENTATION_BOUNDARY.md) |
| Understand host/LLM exchange ownership | [AI Exchange design note](docs/AI_EXCHANGE.md) |
| Define and inspect a production grammar | [Examples](docs/EXAMPLES.md) |
| See protocol identities become grammar structure | [Gallery](docs/GALLERY.md) |
| Define bounded observer conditions and gate-transition intent | [Observer grammar](docs/OBSERVER_GRAMMAR.md) |

## Theory learning path

1. [From vocabulary to language](docs/theory/01-why-grammar.md)
2. [The grammar model](docs/theory/02-grammar-model.md)
3. [Productions, recursion, and reachability](docs/theory/03-productions-recursion.md)
4. [Structural validation and ownership boundaries](docs/theory/04-validation-and-ownership.md)
5. [Worked example: a small command language](docs/theory/05-worked-example.md)
6. [Advanced implications and architectural boundaries](docs/theory/06-advanced-implications.md)

The theory pages explain concepts and map claims to current source and tests. Operational setup remains in the consuming guide; public type contracts remain in the class guides below.

## Source-level class guides

| Type | Contract and purpose | UML and use cases |
|---|---|---|
| `GrammarBuilder` | [Class guide](GrammarBuilder.md) | [UML](GrammarBuilderUML.md) |
| `GrammarDefinition` | [Class guide](GrammarDefinition.md) | [UML](GrammarDefinitionUML.md) |
| `GrammarRule` | [Class guide](GrammarRule.md) | [UML](GrammarRuleUML.md) |
| `GrammarSymbol` | [Class guide](GrammarSymbol.md) | [UML](GrammarSymbolUML.md) |
| `GrammarProtocolReference` | [Class guide](GrammarProtocolReference.md) | [UML](GrammarProtocolReferenceUML.md) |

## AI agent onboarding

Before changing this repository, read [AGENTS.md](AGENTS.md) and the [shared FSM_COS Documentation Standard](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/development/DOCUMENTATION_STANDARD.md).

## Where it fits

- [ProtocolAi](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi) owns vocabulary/category and member-symbol identities.
- [CommandAi](https://github.com/TrentBest/TheSingularityWorkshop.CommandAi) owns command proposal representations and may carry an optional opaque GrammarAi body in its enriched form; it does not validate that body.
- [HeadlessAi](https://github.com/TrentBest/TheSingularityWorkshop.HeadlessAi) provides configured AI endpoint communication.
- [FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS) composes reusable MicroBundle capabilities into a runtime assembly.
- [SingularityBot](https://github.com/TrentBest/TheSingularityWorkshop.SingularityBot) is currently a CommandAi consumer; GrammarAi remains an optional authoring/interchange candidate, not a bot runtime dependency.

GrammarAi does not call a model, parse arbitrary generated text, authorize actions, or execute commands. The host binds external references to the exact request/catalog snapshot, supplies domain meaning, applies policy, and decides what happens.

**Explore the Workshop:** [Find packages on NuGet](https://www.nuget.org/packages?q=TheSingularityWorkshop) · [Browse the repository guide](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/development/docs/WORKSHOP_REPOSITORIES.md).

---

<p align="center"><a href="https://github.com/TrentBest">GitHub</a> · <a href="https://coderlegion.com/">Coder Legion</a> · <a href="https://www.patreon.com/c/TheSingularityWorkshop">Patreon</a></p>

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
