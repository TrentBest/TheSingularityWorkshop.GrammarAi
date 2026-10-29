<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# GrammarAi problem domain

## The human problem

A vocabulary tells a system what a symbol means, but it does not say which combinations are valid. Without a structural contract, a model can return individually familiar identifiers in an invalid order, omit required structure, or combine symbols in a way the consumer cannot interpret.

GrammarAi describes legal composition using a start symbol, nonterminals, and ordered production rules. It is a grammar-definition and structural-validation domain—not a natural-language parser or model client.

## Domain model

- **Grammar definition** — an immutable grammar identity, name, start symbol, and ordered rules.
- **Rule** — a uniquely identified production from one nonterminal to an ordered right-hand side.
- **Symbol** — either a grammar nonterminal or a reference to an externally owned protocol symbol.
- **Protocol reference** — a pair of protocol and symbol identities; the referenced vocabulary remains owned elsewhere.
- **Builder** — the authoring surface for defining the grammar before consumption.

## Responsibilities

GrammarAi validates its own declared structure: the start symbol must have a production, each referenced nonterminal must be defined, and rule identities must be unique. It emits a deterministic description of the grammar so a host or adapter can present the structure to another component.

GrammarAi does not own symbol meaning, infer a parse from free-form text, compile provider-specific constrained-decoding schemas, negotiate protocol versions, invoke a model, authorize actions, or execute commands. A valid grammar definition is not proof that a generated output follows the grammar.

## Boundary with the other AI packages

- **ProtocolAi** owns vocabulary identities and the meaning assigned to its symbols.
- **GrammarAi** owns production rules and structural composition.
- **CommandAi** owns the versioned representation of a command proposal.
- **HeadlessAi** owns configured endpoint request/response communication.
- **The host** decides which definitions to trust, how to validate outputs, and what actions may be taken.

These boundaries describe intended ownership. Do not infer an existing project dependency or end-to-end adapter merely because two package domains are related.

## Acceptance criteria

1. Grammar identity, name, and start symbol are explicit and non-zero.
2. Rule identities are unique.
3. The start symbol is defined by a production.
4. Every referenced nonterminal has a production.
5. Protocol references preserve external ownership instead of copying or redefining vocabulary.
6. Description output is deterministic for the same grammar definition.
7. The package remains independent of model providers, endpoint transport, GUI frameworks, and execution policy.

## Evidence and maturity

The current source model is represented by GrammarDefinition, GrammarRule, GrammarSymbol, GrammarProtocolReference, and GrammarBuilder. The current alpha boundary does not establish grammar parsing, grammar compilation, provider-specific export, constrained decoding, or protocol negotiation. See [Consuming GrammarAi](CONSUMING.md), [Examples](EXAMPLES.md), and [Theory](THEORY.md) for the practical and architectural views.
