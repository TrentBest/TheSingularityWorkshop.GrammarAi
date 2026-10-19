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

## 5. Inspect the grammar

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

## 6. Build a nested structure

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

## 7. Where GrammarAI fits

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

The current alpha provides the vocabulary-reference and grammar representations. The adapter, model integration, validation policy, and execution layers remain outside this package.

## 8. Complete small example

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

## What you still provide

- Protocol definitions.
- Model integration and grammar compilation or provider translation.
- Generated-output parsing and validation policy beyond the current definition checks.
- Execution semantics, persistence, and version negotiation.

GrammarAI stays a reusable structural artifact: **ProtocolAI defines WHAT; GrammarAI defines HOW; the host owns execution.**

For the architectural reasoning and current limits, see [Theory](THEORY.md), [Examples](EXAMPLES.md), and [Reflection](REFLECTION.md).
