<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# GrammarAi Theory — Learning Path

GrammarAi sits after semantic identity and before host policy. ProtocolAi establishes **WHAT** a known value means; GrammarAi establishes **HOW** known identities may be arranged. The host remains responsible for external linking, candidate validation, authorization, and execution.

This is a structural contract—not a claim that GrammarAi constrains a model, parses generated text, or executes a command by itself.

## Knowledge mapping direction\n\nThe intended domain is an integer-first knowledge map, not a compiler. See [Knowledge Mapping Model](KNOWLEDGE_MAPPING_MODEL.md) for the ProtocolAi identity boundary, GrammarAi structural roles, host-owned identifier lifecycle, compact proposal exchange, and the conceptual C# control-flow example.\n\n## Textbook chapters

Read these chapters in order. Each chapter introduces a concept, explains its practical consequence, and points to the source and tests that demonstrate the current implementation.

| Chapter | Subject | What you should learn |
|---|---|---|
| [1. From vocabulary to language](theory/01-why-grammar.md) | Protocol vocabulary versus grammar | Why identity and arrangement are separate concerns |
| [2. The grammar model](theory/02-grammar-model.md) | Definitions, productions, symbols, builder | How the public structural model fits together |
| [3. Productions, recursion, and reachability](theory/03-productions-recursion.md) | Alternatives, sequences, epsilon, recursion, graph reachability | How to read and inspect grammar structure |
| [4. Structural validation and ownership boundaries](theory/04-validation-and-ownership.md) | Construction invariants, resource bounds, external references | What GrammarAi checks and what remains host-owned |
| [5. Worked example: a small command language](theory/05-worked-example.md) | End-to-end structural example | How a consumer builds a grammar without confusing it with authorization or execution |\n| [6. Advanced implications and architectural boundaries](theory/06-advanced-implications.md) | Deprobabilization, CFG foundations, provider neutrality, ownership, and open questions | What deterministic structure can contribute—and what it cannot guarantee |

## Architectural model

```text
ProtocolAi
  WHAT / semantic identity
       |
       v
GrammarAi
  HOW / structural composition
       |
       v
Host-owned linking and candidate validation
       |
       v
Host policy and authorization
       |
       v
Host execution
```

GrammarAi owns grammar identity, nonterminals, ordered productions, and references to external protocol symbols. It does not own the meaning of those protocol symbols. A terminal reference is an identity link, not proof that the referenced protocol or symbol exists in a selected external definition.

## Implementation and evidence map

| Concern | Current implementation | Test evidence |
|---|---|---|
| Grammar construction and invariants | `GrammarBuilder.Build`, `GrammarDefinition` | `GrammarDefinitionTests.cs`: missing start/nonterminal, duplicate IDs, bounds |
| Ordered productions and symbol kinds | `GrammarRule`, `GrammarSymbol` | external-symbol and uninitialized-symbol tests |
| Immutable built snapshot | `GrammarDefinition.Rules` | `Builder_snapshot_is_immutable_after_subsequent_builder_changes` |
| Deterministic description | `GrammarDefinition.Describe` | `Description_is_deterministic` |
| Epsilon representation | Empty `GrammarRule.RightHandSide` | `Empty_production_is_explicit_epsilon` |
| Reachability | `GrammarDefinition.FindUnreachableNonTerminals` | reachability, recursion, and declaration-order tests |
| External protocol-link discovery | `GrammarDefinition.FindUnresolvedProtocolReferences` with a host-supplied resolver | distinct references, first-use order, resolved/unresolved results, null resolver, exception propagation |
| Resource bounds | `GrammarLimits` | bounded-name, rule, per-rule symbol, and aggregate-symbol tests |

The evidence paths above are in `tests/GrammarAi.Tests/GrammarDefinitionTests.cs`. The test names and current source should be treated as the authority when documentation and implementation appear to differ.

## Use theory separately from operational guidance

- [Documentation index](../../DOCUMENTATION_INDEX.md) — complete navigation.
- [Consuming GrammarAi](../CONSUMING.md) — package use and operational guidance.
- [Examples](../EXAMPLES.md) — practical construction examples.
- [Public class guides](../../DOCUMENTATION_INDEX.md#source-level-class-guides) — API contracts and UML diagrams.
- [Problem domain](../PROBLEM_DOMAIN.md) — the problem boundary GrammarAi addresses.

## Architectural invariant

> **GrammarAi describes how protocol identities may be connected without owning their meaning, the model that generated them, or the execution that follows.**

```text
ProtocolAi = WHAT
GrammarAi  = HOW
Host       = POLICY + EXECUTION
```

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
