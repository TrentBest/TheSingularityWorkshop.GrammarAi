<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# Consuming GrammarAI

GrammarAI is consumed by the application or tool that owns the **structure** of an AI-facing protocol.

It does not call a model, parse generated text, execute tools, or provide a provider-specific grammar compiler in the current alpha.

> **Define an integer-backed grammar that describes how externally owned protocol symbols may be connected.**

## 1. Install the package

```bash
dotnet add package TheSingularityWorkshop.GrammarAi --version 0.1.0-alpha.2
```

Or add this to your project file:

```xml
<PackageReference Include="TheSingularityWorkshop.GrammarAi" Version="0.1.0-alpha.2" />
```

Use the version currently published on NuGet when consuming a later release.

## 2. Start with protocol identities

GrammarAI deliberately does not need a direct ProtocolAI package reference. Instead, it refers to externally owned symbols by:

```text
[protocolId:symbolId]
```

For example, `[1001:2001]` means protocol 1001 owns symbol 2001. The protocol remains owned elsewhere.

## 3. Define a grammar

Suppose ProtocolAI owns these identities:

```text
[1001] People
[2001] Bob
[2002] Jane
```

The grammar can define a Greeting structure:

```csharp
using TheSingularityWorkshop.GrammarAi;

var grammar = new GrammarBuilder(
        3001,
        "Greeting",
        4001)
    .Rule(
        5001,
        4001,
        GrammarSymbol.Terminal(
            new GrammarProtocolReference(1001, 2001)))
    .Rule(
        5002,
        4001,
        GrammarSymbol.Terminal(
            new GrammarProtocolReference(1001, 2002)))
    .Build();
```

This means:

```text
[3001] Greeting
    start = [4001]
    [5001] [4001] -> [1001:2001]
    [5002] [4001] -> [1001:2002]
```

GrammarAI owns the grammar IDs and rules. Protocol 1001 owns symbols 2001 and 2002.

## 4. Understand the two symbol types

### Nonterminal

A nonterminal belongs to the grammar:

```csharp
var greeting = GrammarSymbol.NonTerminal(4001);
```

A referenced nonterminal must have a production rule.

### Protocol terminal

A protocol terminal belongs to an external vocabulary:

```csharp
var bob = GrammarSymbol.Terminal(
    new GrammarProtocolReference(1001, 2001));
```

The grammar references Bob; it does not redefine Bob.

## 5. Check external protocol links

Building a grammar validates its internal structure, but it cannot tell whether an external protocol registry actually contains every referenced terminal. The host owns that registry, so pass a lookup into `FindUnresolvedProtocolReferences`:

```csharp
var unresolved = grammar.FindUnresolvedProtocolReferences(reference =>
    protocolRegistry.ContainsSymbol(reference.ProtocolId, reference.SymbolId));

if (unresolved.Count > 0)
{
    // Report missing protocol/symbol identities before using the grammar.
}
```

Here, `protocolRegistry.ContainsSymbol` represents your own lookup API; it is not a GrammarAI method.

The result contains each unresolved `GrammarProtocolReference` once, in first-use order. If the host can resolve every reference, the result is empty. A resolver exception is allowed to propagate so the host can handle registry failures according to its own policy.

This is an identity-link check, not a protocol-version compatibility check, semantic validation, grammar compiler, or generated-output validator. GrammarAI stays independent of ProtocolAI and does not load protocol definitions itself.

## 6. Inspect the grammar

Grammar definitions are self-describing:

```csharp
Console.WriteLine(grammar.Describe());
```

Example output:

```text
[3001] Greeting start=[4001]
  rule [5001] [4001] -> [1001:2001]
  rule [5002] [4001] -> [1001:2002]
```

This is useful for diagnostics, documentation, tests, and future translation layers.

## 7. Build a nested structure

A grammar can connect nonterminals as well as protocol terminals:

```csharp
var grammar = new GrammarBuilder(
        3001,
        "Greeting",
        4001)
    .Rule(5001, 4001, GrammarSymbol.NonTerminal(4002))
    .Rule(
        5002,
        4002,
        GrammarSymbol.Terminal(
            new GrammarProtocolReference(1001, 2001)))
    .Build();
```

Conceptually:

```text
[4001]
  |
  +--> [4002]
          |
          +--> [1001:2001]
```

The grammar defines the relationship. The protocol still owns the terminal.

## 8. Where GrammarAI fits

```text
domain meaning
      |
      v
ProtocolAI — WHAT
      |
      v
GrammarAI — HOW
      |
      v
provider / host adapter
      |
      v
model and host validation
      |
      v
execution
```

The current alpha provides the vocabulary-reference and grammar representations, including an optional host-driven check that external references exist. The adapter, model integration, compatibility policy, and execution layers remain outside this package.

## 9. Complete small example

```csharp
using TheSingularityWorkshop.GrammarAi;

const ulong peopleProtocol = 1001;
const ulong bob = 2001;
const ulong jane = 2002;

var grammar = new GrammarBuilder(3001, "Greeting", 4001)
    .Rule(5001, 4001,
        GrammarSymbol.Terminal(new GrammarProtocolReference(peopleProtocol, bob)))
    .Rule(5002, 4001,
        GrammarSymbol.Terminal(new GrammarProtocolReference(peopleProtocol, jane)))
    .Build();

Console.WriteLine(grammar);
```

Output:

```text
[3001] Greeting start=[4001]
  rule [5001] [4001] -> [1001:2001]
  rule [5002] [4001] -> [1001:2002]
```

## What GrammarAI gives you

- Grammar identity and start symbol.
- Integer nonterminals and ordered production rules.
- References to externally owned protocol symbols.
- Structural validation and deterministic self-description.
- Host-driven discovery of missing external protocol/symbol identities.

## What you still provide

- Protocol definitions and the resolver used to look up their identities.
- Model integration and grammar compilation or provider translation.
- Protocol-version compatibility and generated-output validation policy.
- Execution semantics, persistence, and version negotiation.

GrammarAI stays a reusable structural artifact: **ProtocolAI defines WHAT; GrammarAI defines HOW; the host owns execution.**

For the architectural reasoning and current limits, see [Theory](THEORY.md), [Examples](EXAMPLES.md), and [Reflection](REFLECTION.md).

---

<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://github.com/TrentBest.png?size=200" alt="The Singularity Workshop on GitHub" width="200">
  </a>
</p>

<p align="center">
  <a href="https://github.com/TrentBest">GitHub</a> ·
  <a href="https://coderlegion.com/">Coder Legion</a> ·
  <a href="https://www.patreon.com/c/TheSingularityWorkshop">Patreon</a> ·
  <a href="https://www.paypal.com/donate/?hosted_button_id=3Z7263LCQMV9J">PayPal</a>
  <br>
  <a href="https://github.com/TrentBest/FSM_API">FSM_API</a> ·
  <a href="https://github.com/TrentBest/TheSingularityWorkshop.FSM_Serialization">FSM_Serialization</a> ·
  <a href="https://github.com/TrentBest/WebPage">WebPage</a>
</p>

<p align="center"><em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br><strong>Because state shouldn't be a mess.</strong><br><em>And because static boundaries are invitations to cause trouble.</em></p>
